using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Admin;

namespace Application.Features.Questions;

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, List<QuestionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllQuestionsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<QuestionDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            include: q => q.Include(x => x.Level)
        ); 

        return questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            Content = q.Content,
            Score = q.Score,
            LevelTitle = q.Level.Title,
            ImageUrl = q.ImageUrl
        }).ToList();
    }
}