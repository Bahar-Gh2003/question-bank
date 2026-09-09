using Application.Common;
using Application.Contracts;
using Application.Services;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Student;

namespace Application.Features.Student;

/// <summary>
/// Creates the exam session on the server.
/// This is the only way to start an exam; every eligibility check happens here.
/// </summary>
public class StartExamCommandHandler : IRequestHandler<StartExamCommand, ExamSessionDto>
{
    private const int QuestionsPerExam = 15; // TODO: this should be a field on Exam

    private readonly IUnitOfWork _unitOfWork;
    public StartExamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ExamSessionDto> Handle(StartExamCommand request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(
            request.ExamId, include: q => q.Include(e => e.Level));
        if (exam is null)
            throw new NotFoundException("آزمون یافت نشد.");

        var student = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (student is null)
            throw new NotFoundException("کاربر یافت نشد.");

        // Check 1: the exam must belong to the student's current level
        if (student.CurrentLevelId != exam.LevelId)
            throw new ForbiddenException("این آزمون مربوط به سطح فعلی شما نیست.");

        var attempts = (await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.UserId == request.UserId && a.ExamId == request.ExamId)).ToList();

        // Check 2: resume an open session if one exists (e.g. the user refreshed the page)
        var openAttempt = attempts.FirstOrDefault(a => !a.IsCompleted);
        if (openAttempt is not null)
        {
            var endsAt = openAttempt.StartedAt.AddMinutes(exam.DurationInMinutes);

            if (utcNow < endsAt)
                return await BuildSessionAsync(exam, openAttempt.Id, (int)(endsAt - utcNow).TotalSeconds);

            // Time ran out and it was never submitted -> close it now
            await CloseExpiredAttemptAsync(openAttempt, exam, endsAt, cancellationToken);
            attempts = (await _unitOfWork.ExamAttemptRepository.GetAllAsync(
                predicate: a => a.UserId == request.UserId && a.ExamId == request.ExamId)).ToList();
        }

        // Check 3: attempt count and waiting period - the same rules shown to the user
        var eligibility = ExamEligibilityCalculator.Evaluate(exam, attempts, utcNow);
        if (!eligibility.CanStart)
            throw new BusinessRuleException($"امکان شروع این آزمون وجود ندارد. وضعیت فعلی: {eligibility.Status}");

        // Pick the questions and build the session
        var pool = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == exam.LevelId,
            include: q => q.Include(o => o.Options));

        var selected = pool.OrderBy(_ => Guid.NewGuid()).Take(QuestionsPerExam).ToList();
        if (selected.Count == 0)
            throw new BusinessRuleException("برای این آزمون هنوز سوالی تعریف نشده است.");

        var attempt = new ExamAttempt
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ExamId = exam.Id,
            LevelId = exam.LevelId,
            StartedAt = utcNow,
            AttemptedAt = utcNow,
            IsCompleted = false
        };

        // The chosen questions are stored now so that at submit time
        // the server knows exactly which questions this student was given.
        foreach (var q in selected)
        {
            attempt.StudentAnswers.Add(new StudentAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = q.Id,
                AttemptCount = 0
            });
        }

        await _unitOfWork.ExamAttemptRepository.AddAsync(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildSessionAsync(exam, attempt.Id, exam.DurationInMinutes * 60);
    }

    /// <summary>Closes and scores a session whose time expired without being submitted.</summary>
    private async Task CloseExpiredAttemptAsync(
        ExamAttempt attempt, Exam exam, DateTime endsAt, CancellationToken cancellationToken)
    {
        var full = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            attempt.Id, include: a => a.Include(x => x.StudentAnswers));
        if (full is null) return;

        var questionIds = full.StudentAnswers.Select(a => a.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id));

        full.Score = full.StudentAnswers
            .Where(a => a.IsCorrect == true)
            .Sum(a => questions.First(q => q.Id == a.QuestionId).Score);

        full.IsPassed = full.Score >= exam.PassingScore;
        full.AttemptedAt = endsAt;
        full.IsCompleted = true;

        _unitOfWork.ExamAttemptRepository.Update(full);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ExamSessionDto> BuildSessionAsync(Exam exam, Guid attemptId, int remainingSeconds)
    {
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            attemptId, include: a => a.Include(x => x.StudentAnswers));

        var questionIds = attempt!.StudentAnswers.Select(a => a.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id),
            include: q => q.Include(o => o.Options));

        var dtos = attempt.StudentAnswers.Select(sa =>
        {
            var q = questions.First(x => x.Id == sa.QuestionId);
            return new ExamQuestionDto
            {
                QuestionId = q.Id,
                Content = q.Content,
                Type = q.Type,
                ImageUrl = q.ImageUrl,
                AttemptsUsed = sa.AttemptCount,
                IsLocked = sa.IsCorrect == true || sa.AttemptCount >= 2,
                SelectedOptionId = sa.SelectedOptionId,
                ShortAnswerText = sa.ShortAnswerText,
                Options = q.Type == QuestionType.MultipleChoice
                    // Only the option id and text - IsCorrect is deliberately left unset
                    ? q.Options.Select(o => new OptionDto { Id = o.Id, Content = o.Content }).ToList()
                    : new List<OptionDto>()
            };
        }).ToList();

        return new ExamSessionDto
        {
            AttemptId = attemptId,
            ExamTitle = exam.Title,
            DurationInMinutes = exam.DurationInMinutes,
            RemainingSeconds = Math.Max(0, remainingSeconds),
            Questions = dtos
        };
    }
}