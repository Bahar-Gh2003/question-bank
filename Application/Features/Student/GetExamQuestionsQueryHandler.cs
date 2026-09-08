using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred;
using Sahred.Student;

namespace Application.Features.Student;

public class GetExamQuestionsQueryHandler : IRequestHandler<GetExamQuestionsQuery, ExamSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetExamQuestionsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ExamSessionDto> Handle(GetExamQuestionsQuery request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId);
        if (exam == null) throw new System.Exception("آزمون یافت نشد.");

        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == exam.LevelId,
            include: q => q.Include(o => o.Options)
        );

        var randomQuestions = questions.OrderBy(q => Guid.NewGuid()).Take(15)
            .Select(q => new ExamQuestionDto
            {
                QuestionId = q.Id,
                Content = q.Content,
                Type = q.Type,
                ImageUrl = q.ImageUrl,
                // CorrectAnswer = q.Options.FirstOrDefault(o => o.IsCorrect)?.Content ?? "", // این خط ممکن است دیگر لازم نباشد
                CorrectOptionId = q.Options.FirstOrDefault(o => o.IsCorrect)?.Id ?? Guid.Empty, // <-- این خط را اضافه کنید
                Options = q.Type == QuestionType.MultipleChoice
                    ? q.Options.Select(o => new OptionDto { Id = o.Id, Content = o.Content }).ToList()
                    : new List<OptionDto>()
            }).ToList();

        return new ExamSessionDto
        {
            ExamTitle = exam.Title,
            DurationInMinutes = exam.DurationInMinutes,
            Questions = randomQuestions
        };
    }
}