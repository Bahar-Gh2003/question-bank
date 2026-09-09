using Application.Common;
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
        var attempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: q => q.Include(e => e.Exam).Include(sa => sa.StudentAnswers));

        if (attempt is null)
            throw new NotFoundException("سابقه آزمون یافت نشد.");

        if (attempt.UserId != request.UserId)
            throw new ForbiddenException("شما به این سابقه آزمون دسترسی ندارید.");

        // Only the questions this attempt actually contained. Questions are drawn at
        // random per attempt, so reading the whole level pool would show the student
        // questions they were never asked.
        var questionIds = attempt.StudentAnswers.Select(sa => sa.QuestionId).ToList();
        var questions = (await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id),
            include: q => q.Include(o => o.Options))).ToList();

        var reviewQuestions = attempt.StudentAnswers.Select(answer =>
        {
            var question = questions.First(q => q.Id == answer.QuestionId);

            string studentAnswerText;
            bool? isCorrect;

            if (question.Type == QuestionType.MultipleChoice)
            {
                studentAnswerText = question.Options
                    .FirstOrDefault(o => o.Id == answer.SelectedOptionId)?.Content ?? "پاسخ داده نشده";

                // Graded live during the exam, so this is always known
                isCorrect = answer.IsCorrect ?? false;
            }
            else
            {
                studentAnswerText = string.IsNullOrWhiteSpace(answer.ShortAnswerText)
                    ? "پاسخ داده نشده"
                    : answer.ShortAnswerText;

                // Null until the admin grades it; an unanswered question needs no grading
                isCorrect = string.IsNullOrWhiteSpace(answer.ShortAnswerText)
                    ? false
                    : answer.IsCorrect;
            }

            return new ExamReviewQuestionDto
            {
                Content = question.Content,
                ImageUrl = question.ImageUrl,
                Type = question.Type,
                Options = question.Options
                    .Select(o => new OptionDto { Id = o.Id, Content = o.Content, IsCorrect = o.IsCorrect })
                    .ToList(),
                StudentAnswer = studentAnswerText,
                SelectedOptionId = answer.SelectedOptionId,
                CorrectAnswer = question.Options.FirstOrDefault(o => o.IsCorrect)?.Content ?? "",
                IsStudentAnswerCorrect = isCorrect,
                Score = question.Score
            };
        }).ToList();

        return new ExamReviewDto
        {
            ExamTitle = attempt.Exam.Title,
            AwaitingGrading = !attempt.IsGraded,
            Questions = reviewQuestions
        };
    }
}