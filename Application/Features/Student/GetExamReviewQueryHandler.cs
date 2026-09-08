using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Student;

namespace Application.Features.Student;

public class GetExamReviewQueryHandler : IRequestHandler<GetExamReviewQuery, ExamReviewDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetExamReviewQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ExamReviewDto> Handle(GetExamReviewQuery request, CancellationToken cancellationToken)
    {
         var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(request.AttemptId,
            include: q => q.Include(e => e.Exam).Include(sa => sa.StudentAnswers));

        if (attempt == null || attempt.UserId != request.UserId)
            throw new Exception("سابقه آزمون یافت نشد.");

        // ۱. تمام سوالات آزمونی که دانشجو در آن شرکت کرده را می‌خوانیم
        var allExamQuestions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == attempt.Exam.LevelId,
            include: q => q.Include(o => o.Options)
        );

        // ۲. حالا برای هر سوال، پاسخ دانشجو را پیدا می‌کنیم (اگر وجود داشته باشد)
        var reviewQuestions = allExamQuestions.Select(q =>
        {
            var studentAnswerRecord = attempt.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == q.Id);
            string studentAnswerText = "پاسخ داده نشده";
            Guid? selectedOptionId = null;
            bool isCorrect = false;

            if (studentAnswerRecord != null) // اگر دانشجو پاسخی ثبت کرده بود
            {
                if (q.Type == QuestionType.MultipleChoice)
                {
                    selectedOptionId = studentAnswerRecord.SelectedOptionId;
                    studentAnswerText = q.Options.FirstOrDefault(o => o.Id == selectedOptionId)?.Content ?? "پاسخ داده نشده";
                    var correctOptionId = q.Options.FirstOrDefault(o => o.IsCorrect)?.Id;
                    isCorrect = selectedOptionId.HasValue && selectedOptionId == correctOptionId;
                }
                else
                {
                    studentAnswerText = studentAnswerRecord.ShortAnswerText ?? "پاسخ داده نشده";
                }
            }

            return new ExamReviewQuestionDto
            {
                Content = q.Content,
                ImageUrl = q.ImageUrl,
                Type = q.Type,
                Options = q.Options.Select(o => new OptionDto { Id = o.Id, Content = o.Content, IsCorrect = o.IsCorrect }).ToList(),
                StudentAnswer = studentAnswerText,
                SelectedOptionId = selectedOptionId,
                CorrectAnswer = q.Options.FirstOrDefault(o => o.IsCorrect)?.Content ?? "",
                IsStudentAnswerCorrect = isCorrect,
                Score = q.Score,
            };
        }).ToList();

        return new ExamReviewDto
        {
            ExamTitle = attempt.Exam.Title,
            Questions = reviewQuestions
        };
        }
}