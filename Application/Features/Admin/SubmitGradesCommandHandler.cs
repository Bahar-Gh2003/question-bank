using Application.Common;
using Application.Contracts;
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
        // 🔧 اصلاح باگ: Level هم باید Include شود، چون پایین‌تر
        // به attempt.Exam.Level.LevelNumber دسترسی داریم.
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: q => q.Include(e => e.Exam).ThenInclude(e => e.Level)
                           .Include(sa => sa.StudentAnswers));

        if (attempt is null)
            throw new NotFoundException("سابقه آزمون یافت نشد.");

        // ۱. وضعیت IsCorrect سوالات تشریحی را طبق نظر ادمین به‌روز می‌کنیم
        foreach (var gradedAnswer in request.Dto.GradedAnswers)
        {
            var answerToUpdate = attempt.StudentAnswers
                .FirstOrDefault(sa => sa.QuestionId == gradedAnswer.Key);

            if (answerToUpdate is not null)
                answerToUpdate.IsCorrect = gradedAnswer.Value;
        }

        // ۲. نمره نهایی را دوباره روی تمام پاسخ‌ها حساب می‌کنیم
        var questionIds = attempt.StudentAnswers.Select(sa => sa.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id));

        var finalScore = attempt.StudentAnswers
            .Where(sa => sa.IsCorrect == true)
            .Sum(sa => questions.First(q => q.Id == sa.QuestionId).Score);

        attempt.Score = finalScore;
        attempt.IsPassed = finalScore >= attempt.Exam.PassingScore;

        // ۳. ارتقای سطح در صورت قبولی
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
    }
}