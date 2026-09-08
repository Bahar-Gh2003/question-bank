using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Command;

namespace Application.Features.Questions;

public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, UpdateQuestionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetQuestionByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<UpdateQuestionCommand> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _unitOfWork.QuestionRepository.GetByIdAsync(request.Id, 
            include: q => q.Include(o => o.Options)); 
        
        if (question == null) throw new System.Exception("سوال یافت نشد.");

        var command = new UpdateQuestionCommand
        {
            Id = question.Id,
            Content = question.Content,
            Type = question.Type,
            Score = question.Score,
            LevelId = question.LevelId,
            ImageUrl = question.ImageUrl,
            Options = question.Options.Select(o => new OptionDto
            {
                Content = o.Content,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        if (question.Type == QuestionType.ShortAnswer)
        {
            command.CorrectAnswer = question.Options.FirstOrDefault(o => o.IsCorrect)?.Content ?? "";
        }

        return command;
    }
}