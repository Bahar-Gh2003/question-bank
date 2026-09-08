using Application.Common;
using Application.Contracts;
using Application.Services;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred;
using Sahred.Student;

namespace Application.Features.Student;

/// <summary>
/// جلسه آزمون را در سرور می‌سازد.
/// این تنها راه شروع آزمون است و همه بررسی‌های مجوز اینجا انجام می‌شود.
/// </summary>
public class StartExamCommandHandler : IRequestHandler<StartExamCommand, ExamSessionDto>
{
    private const int QuestionsPerExam = 15; // TODO: بهتر است فیلدی روی Exam باشد

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

        // بررسی ۱: آزمون باید متعلق به سطح فعلی دانشجو باشد
        if (student.CurrentLevelId != exam.LevelId)
            throw new ForbiddenException("این آزمون مربوط به سطح فعلی شما نیست.");

        var attempts = (await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.UserId == request.UserId && a.ExamId == request.ExamId)).ToList();

        // بررسی ۲: اگر جلسه بازی وجود دارد، همان را ادامه بده (مثلاً کاربر صفحه را رفرش کرده)
        var openAttempt = attempts.FirstOrDefault(a => !a.IsCompleted);
        if (openAttempt is not null)
        {
            var endsAt = openAttempt.StartedAt.AddMinutes(exam.DurationInMinutes);

            if (utcNow < endsAt)
                return await BuildSessionAsync(exam, openAttempt.Id, (int)(endsAt - utcNow).TotalSeconds);

            // وقتش تمام شده بوده و هرگز ثبت نشده → همین حالا ببندش
            await CloseExpiredAttemptAsync(openAttempt, exam, endsAt, cancellationToken);
            attempts = (await _unitOfWork.ExamAttemptRepository.GetAllAsync(
                predicate: a => a.UserId == request.UserId && a.ExamId == request.ExamId)).ToList();
        }

        // بررسی ۳: قوانین تعداد تلاش و زمان انتظار — همان منطقی که به کاربر نشان داده می‌شود
        var eligibility = ExamEligibilityCalculator.Evaluate(exam, attempts, utcNow);
        if (!eligibility.CanStart)
            throw new BusinessRuleException($"امکان شروع این آزمون وجود ندارد. وضعیت فعلی: {eligibility.Status}");

        // انتخاب سوال‌ها و ساخت جلسه
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

        // سوال‌های انتخاب‌شده همین حالا ذخیره می‌شوند تا موقع ثبت،
        // سرور دقیقاً بداند کدام سوال‌ها به این دانشجو داده شده بود.
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

    /// <summary>جلسه‌ای که وقتش تمام شده اما هرگز ثبت نشده را می‌بندد و نمره می‌دهد.</summary>
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
                    // فقط Id و متن گزینه — IsCorrect عمداً پر نمی‌شود
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