using System.Text.Json;
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
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(request.AttemptId, 
            include: q => q.Include(e => e.Exam).Include(sa => sa.StudentAnswers));

        if (attempt == null) throw new Exception("سابقه آزمون یافت نشد.");

        // ۲. ابتدا وضعیت IsCorrect را برای سوالات تشریحی که ادمین تصحیح کرده، در دیتابیس آپدیت می‌کنیم
        foreach (var gradedAnswer in request.Dto.GradedAnswers)
        {
            var answerToUpdate = attempt.StudentAnswers
                .FirstOrDefault(sa => sa.QuestionId == gradedAnswer.Key);
            if (answerToUpdate != null)
            {
                answerToUpdate.IsCorrect = gradedAnswer.Value;
            }
        }

        // ۳. حالا نمره نهایی را دوباره بر اساس "تمام" پاسخ‌ها محاسبه می‌کنیم
        int finalScore = 0;
        var questionIds = attempt.StudentAnswers.Select(sa => sa.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id)
        );

        // این حلقه حالا هم سوالات تستی (که IsCorrect آنها از قبل true بوده) 
        // و هم تشریحی‌های تصحیح شده را در نظر می‌گیرد
        foreach (var studentAnswer in attempt.StudentAnswers)
        {
            if (studentAnswer.IsCorrect == true)
            {
                finalScore += questions.First(q => q.Id == studentAnswer.QuestionId).Score;
            }
            // var question = questions.First(q => q.Id == studentAnswer.QuestionId);
            // bool isAnswerCorrect = false;
            //
            // if (question.Type == QuestionType.MultipleChoice)
            // {
            //     // Re-evaluate the multiple-choice answer to be safe
            //     var correctOptionId = question.Options.FirstOrDefault(o => o.IsCorrect)?.Id;
            //     isAnswerCorrect = studentAnswer.SelectedOptionId == correctOptionId;
            // }
            // else // For short-answer, use the value we just updated
            // {
            //     isAnswerCorrect = studentAnswer.IsCorrect ?? false;
            // }

            // if (isAnswerCorrect)
            // {
            //     finalScore += question.Score;
            // }
        }

        // ۴. نمره نهایی و وضعیت قبولی را در رکورد اصلی تلاش، آپدیت می‌کنیم
        attempt.Score = finalScore;
        attempt.IsPassed = finalScore >= attempt.Exam.PassingScore;

        if (attempt.IsPassed)
        {
            var student = await _unitOfWork.UserRepository.GetByIdAsync(attempt.UserId);
            var nextLevel = await _unitOfWork.LevelRepository.FindFirstOrDefaultAsync(
                l => l.LevelNumber == attempt.Exam.Level.LevelNumber + 1
            );

            if (student != null && nextLevel != null)
            {
                student.CurrentLevelId = nextLevel.Id;
                _unitOfWork.UserRepository.Update(student);
            }
        }
        
        _unitOfWork.ExamAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}