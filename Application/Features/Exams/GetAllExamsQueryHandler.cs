using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Admin;

namespace Application.Features.Exams;

public class GetAllExamsQueryHandler : IRequestHandler<GetAllExamsQuery, List<AdminExamListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllExamsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<AdminExamListDto>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
    {
        var exams = await _unitOfWork.ExamRepository.GetAllAsync(include: e => e.Include(l => l.Level));
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync();

        // How many questions each level has, so the admin can see when an exam
        // asks for more questions than exist.
        var questionsPerLevel = questions
            .GroupBy(q => q.LevelId)
            .ToDictionary(g => g.Key, g => g.Count());

        return exams.Select(e => new AdminExamListDto
            {
                Id = e.Id,
                Title = e.Title,
                LevelTitle = e.Level!.Title,
                DurationInMinutes = e.DurationInMinutes,
                QuestionCount = e.QuestionCount,
                AvailableQuestionCount = questionsPerLevel.GetValueOrDefault(e.LevelId, 0)
            })
            .OrderBy(e => e.LevelTitle)
            .ThenBy(e => e.Title)
            .ToList();
    }
}