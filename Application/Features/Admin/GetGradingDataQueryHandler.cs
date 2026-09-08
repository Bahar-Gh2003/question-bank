using System.Text.Json;
using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Admin;

namespace Application.Features.Admin;

public class GetGradingDataQueryHandler : IRequestHandler<GetGradingDataQuery, List<GradingQuestionDto>>
{
     private readonly IUnitOfWork _unitOfWork;

    public GetGradingDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GradingQuestionDto>> Handle(GetGradingDataQuery request, CancellationToken cancellationToken)
    {
        // ما سابقه آزمون را به همراه پاسخ‌های دانشجو و سوالات مربوطه می‌خوانیم
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(request.AttemptId,
            include: q => q.Include(ea => ea.StudentAnswers)
                           .ThenInclude(sa => sa.Question)
                           .ThenInclude(qu => qu.Options)
        );

        if (attempt == null) throw new Exception("سابقه آزمون یافت نشد.");

        var gradingList = new List<GradingQuestionDto>();
        
        // حالا روی پاسخ‌های ذخیره شده در دیتابیس پیمایش می‌کنیم
        foreach (var studentAnswer in attempt.StudentAnswers)
        {
           
            var question = studentAnswer.Question;
            var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect)?.Content ?? "";
            string studentAnswerText = "";

            if (question.Type == QuestionType.MultipleChoice)
            {
                studentAnswerText = question.Options.FirstOrDefault(o => o.Id == studentAnswer.SelectedOptionId)?.Content ?? "پاسخ داده نشده";
            }
            else
            {
                studentAnswerText = studentAnswer.ShortAnswerText ?? "پاسخ داده نشده";
            }

            gradingList.Add(new GradingQuestionDto
            {
                QuestionId = question.Id,
                Content = question.Content,
                ImageUrl = question.ImageUrl,
                Type = question.Type,
                CorrectAnswer = correctAnswer,
                StudentAnswer = studentAnswerText,
                Score = question.Score,
                IsCorrect = studentAnswer.IsCorrect ,
                // IsCorrect = (question.Type == QuestionType.MultipleChoice) 
                // ? (studentAnswerText == correctAnswer) 
                    // : null
            });
        }

        return gradingList;
    }
}