using Application.Common;
using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Student;

/// <summary>
/// Holds the logic that used to live in TakeExam.razor.
/// Both answer grading and retry counting happen on the server,
/// so a student can neither see the correct answer nor retry more than twice.
/// </summary>
public class CheckAnswerCommandHandler : IRequestHandler<CheckAnswerCommand, CheckAnswerResultDto>
{
    private const int MaxTriesPerQuestion = 2;

    private readonly IUnitOfWork _unitOfWork;
    public CheckAnswerCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CheckAnswerResultDto> Handle(CheckAnswerCommand request, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: a => a.Include(x => x.StudentAnswers).Include(x => x.Exam));

        if (attempt is null)
            throw new NotFoundException("جلسه آزمون یافت نشد.");

        // Ownership check: a student may only touch their own session
        if (attempt.UserId != request.UserId)
            throw new ForbiddenException("شما به این جلسه آزمون دسترسی ندارید.");

        if (attempt.IsCompleted)
            throw new BusinessRuleException("این آزمون قبلاً ثبت شده است.");

        // Server-side time check - the client timer cannot be trusted
        var endsAt = attempt.StartedAt.AddMinutes(attempt.Exam.DurationInMinutes);
        if (DateTime.UtcNow > endsAt)
            throw new BusinessRuleException("زمان آزمون به پایان رسیده است.");

        var answer = attempt.StudentAnswers.FirstOrDefault(a => a.QuestionId == request.QuestionId);
        if (answer is null)
            throw new BusinessRuleException("این سوال جزو سوالات آزمون شما نیست.");

        // The question is already locked
        if (answer.IsCorrect == true || answer.AttemptCount >= MaxTriesPerQuestion)
        {
            return new CheckAnswerResultDto
            {
                IsCorrect = answer.IsCorrect == true,
                AttemptsUsed = answer.AttemptCount,
                IsLocked = true,
                Message = "این سوال قفل شده است."
            };
        }

        var question = await _unitOfWork.QuestionRepository.GetByIdAsync(
            request.QuestionId, include: q => q.Include(o => o.Options));
        if (question is null)
            throw new NotFoundException("سوال یافت نشد.");

        // The submitted option must actually belong to this question
        if (question.Options.All(o => o.Id != request.SelectedOptionId))
            throw new BusinessRuleException("گزینه انتخابی معتبر نیست.");

        var correctOptionId = question.Options.FirstOrDefault(o => o.IsCorrect)?.Id;
        var isCorrect = correctOptionId is not null && request.SelectedOptionId == correctOptionId;

        answer.AttemptCount++;
        answer.SelectedOptionId = request.SelectedOptionId;
        answer.IsCorrect = isCorrect;

        _unitOfWork.StudentAnswerRepository.Update(answer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var isLocked = isCorrect || answer.AttemptCount >= MaxTriesPerQuestion;

        return new CheckAnswerResultDto
        {
            IsCorrect = isCorrect,
            AttemptsUsed = answer.AttemptCount,
            IsLocked = isLocked,
            Message = isCorrect
                ? "آفرین! پاسخ شما صحیح است."
                : isLocked
                    ? "پاسخ شما صحیح نبود. این سوال قفل شد."
                    : "پاسخ شما صحیح نبود. یک بار دیگر فرصت دارید."
        };
    }
}