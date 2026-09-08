using Application.Common;
using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Student;

/// <summary>
/// آزمون را نهایی می‌کند. کلاینت فقط AttemptId و پاسخ‌های تشریحی را می‌فرستد؛
/// پاسخ‌های تستی از قبل در سرور ذخیره شده‌اند و دوباره از کلاینت پذیرفته نمی‌شوند.
/// </summary>
public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, ExamResultDto>
{
    /// <summary>چند ثانیه ارفاق برای تأخیر شبکه هنگام ثبت خودکار در لحظه پایان.</summary>
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

        // پاسخ‌های تشریحی فقط اگر در مهلت باشند پذیرفته می‌شوند
        if (!isLate)
        {
            foreach (var (questionId, text) in request.ShortAnswerTexts)
            {
                var answer = attempt.StudentAnswers.FirstOrDefault(a => a.QuestionId == questionId);
                if (answer is null) continue;  // سوالی که جزو این آزمون نبوده، نادیده گرفته می‌شود
                answer.ShortAnswerText = text;
            }
        }

        // نمره‌دهی فقط بر اساس داده‌های سرور
        var questionIds = attempt.StudentAnswers.Select(a => a.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id));

        var totalScore = attempt.StudentAnswers
            .Where(a => a.IsCorrect == true)
            .Sum(a => questions.First(q => q.Id == a.QuestionId).Score);

        attempt.Score = totalScore;
        attempt.IsPassed = totalScore >= attempt.Exam.PassingScore;
        attempt.AttemptedAt = isLate ? endsAt : utcNow;
        attempt.IsCompleted = true;

        // ارتقای سطح
        if (attempt.IsPassed && attempt.Exam.Level is not null)
        {
            var student = await _unitOfWork.UserRepository.GetByIdAsync(attempt.UserId);
            var nextLevel = await _unitOfWork.LevelRepository.FindFirstOrDefaultAsync(
                l => l.LevelNumber == attempt.Exam.Level.LevelNumber + 1);

            if (student is not null && nextLevel is not null)
            {
                student.CurrentLevelId = nextLevel.Id;
                _unitOfWork.UserRepository.Update(student);
            }
        }

        _unitOfWork.ExamAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExamResultDto { TotalScore = totalScore, IsPassed = attempt.IsPassed };
    }
}