using Application.Common;
using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin;

public class SubmitGradesCommandHandler : IRequestHandler<SubmitGradesCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitGradesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubmitGradesCommand request, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: q => q.Include(e => e.Exam).ThenInclude(e => e.Level)
                           .Include(sa => sa.StudentAnswers));

        if (attempt is null)
            throw new NotFoundException("سابقه آزمون یافت نشد.");

        // 1. Apply the admin's grading to the short-answer questions
        foreach (var gradedAnswer in request.Dto.GradedAnswers)
        {
            var answerToUpdate = attempt.StudentAnswers
                .FirstOrDefault(sa => sa.QuestionId == gradedAnswer.Key);

            if (answerToUpdate is not null)
                answerToUpdate.IsCorrect = gradedAnswer.Value;
        }

        // 2. Recalculate the final score across all answers
        var questionIds = attempt.StudentAnswers.Select(sa => sa.QuestionId).ToList();
        var questions = (await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id))).ToList();

        var finalScore = attempt.StudentAnswers
            .Where(sa => sa.IsCorrect == true)
            .Sum(sa => questions.First(q => q.Id == sa.QuestionId).Score);

        attempt.Score = finalScore;
        attempt.IsPassed = finalScore >= attempt.Exam.PassingScore;
        attempt.IsGraded = true;

        // 3. Promote the student's level if they passed
        if (attempt.IsPassed)
            await PromoteIfEligibleAsync(attempt);

        _unitOfWork.ExamAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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