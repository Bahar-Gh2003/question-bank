using Application.Contracts;
using Domain;
using MediatR;
using Sahred.Command;

namespace Application.Features.Questions;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateQuestionCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!request.LevelId.HasValue)
        {
            throw new Exception("Question level must be selected.");
        }

        var question = new Question
        {
            Content = request.Content,
            Type = request.Type,
            ImageUrl = request.ImageUrl,
            Score = request.Score,
            LevelId = request.LevelId.Value
        };

        if (request.Type == QuestionType.MultipleChoice)
        {
            foreach (var optionDto in request.Options)
            {
                question.Options.Add(new Option
                {
                    Content = optionDto.Content,
                    IsCorrect = optionDto.IsCorrect
                });
            }
        }
        else
        {
            question.Options.Add(new Option
            {
                Content = request.CorrectAnswer,
                IsCorrect = true
            });
        }

        await _unitOfWork.QuestionRepository.AddAsync(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return question.Id;
    }
}