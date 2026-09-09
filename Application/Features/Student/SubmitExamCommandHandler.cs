using Application.Common;
using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Student;

/// <summary>
/// Finalises the exam. The client sends only the AttemptId and short answers;
/// multiple-choice answers are already stored server-side and are not re-accepted.
/// </summary>
public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, ExamResultDto>
{
    /// <summary>Grace period for network latency when auto-submitting at the deadline.</summary>
    private const int GracePeriodSeconds = 30;

    private readonly IUnitOfWork _unitOfWork;
    public SubmitExamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ExamResultDto> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: a => a.Include(x => x.StudentAnswers)
                           .Include(x => x.Exam).ThenInclude(e => e.Level));

        if (attempt is null)
            throw new NotFoundException("جلسه آزمون یافت نشد.");

        if (attempt.UserId != request.UserId)
            throw new ForbiddenException("شما به این جلسه آزمون دسترسی ندارید.");

        if (attempt.IsCompleted)
            throw new BusinessRuleException("این آزمون قبلاً ثبت شده است.");

        var endsAt = attempt.StartedAt.AddMinutes(attempt.Exam.DurationInMinutes);
        var isLate = utcNow > endsAt.AddSeconds(GracePeriodSeconds);

        // Short answers are only accepted if submitted within the time limit
        if (!isLate)
        {
            foreach (var (questionId, text) in request.ShortAnswerTexts)
            {
                var answer = attempt.StudentAnswers.FirstOrDefault(a => a.QuestionId == questionId);
                if (answer is null) continue;  // Ignore questions that were not part of this attempt
                answer.ShortAnswerText = text;
            }
        }

        var questionIds = attempt.StudentAnswers.Select(a => a.QuestionId).ToList();
        var questions = (await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id))).ToList();

        // Does anything still need a human to look at it?
        var needsGrading = attempt.StudentAnswers.Any(a =>
            questions.First(q => q.Id == a.QuestionId).Type == QuestionType.ShortAnswer
            && !string.IsNullOrWhiteSpace(a.ShortAnswerText));

        // Score using server-side data only. While ungraded this covers
        // the multiple-choice questions alone and is therefore provisional.
        var totalScore = attempt.StudentAnswers
            .Where(a => a.IsCorrect == true)
            .Sum(a => questions.First(q => q.Id == a.QuestionId).Score);

        attempt.Score = totalScore;
        attempt.AttemptedAt = isLate ? endsAt : utcNow;
        attempt.IsCompleted = true;
        attempt.IsGraded = !needsGrading;

        // Pass/fail and promotion wait for grading, so a student is never told
        // they failed on the strength of half a score.
        if (attempt.IsGraded)
        {
            attempt.IsPassed = totalScore >= attempt.Exam.PassingScore;

            if (attempt.IsPassed)
                await PromoteIfEligibleAsync(attempt);
        }

        _unitOfWork.ExamAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExamResultDto
        {
            TotalScore = totalScore,
            IsPassed = attempt.IsPassed,
            AwaitingGrading = needsGrading
        };
    }

    /// <summary>
    /// Moves the student up one level, but only when they are actually sitting at
    /// the exam's level. Passing a lower-level exam later must never demote them.
    /// </summary>
    private async Task PromoteIfEligibleAsync(ExamAttempt attempt)
    {
        if (attempt.Exam.Level is null) return;

        var student = await _unitOfWork.UserRepository.GetByIdAsync(attempt.UserId);
        if (student?.CurrentLevelId is null) return;

        var currentLevel = await _unitOfWork.LevelRepository.GetByIdAsync(student.CurrentLevelId.Value);
        if (currentLevel is null) return;

        if (attempt.Exam.Level.LevelNumber != currentLevel.LevelNumber) return;

        var nextLevel = await _unitOfWork.LevelRepository.FindFirstOrDefaultAsync(
            l => l.LevelNumber == currentLevel.LevelNumber + 1);
        if (nextLevel is null) return;

        student.CurrentLevelId = nextLevel.Id;
        _unitOfWork.UserRepository.Update(student);
    }
}