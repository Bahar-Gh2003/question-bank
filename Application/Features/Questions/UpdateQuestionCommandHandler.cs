using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Command;

namespace Application.Features.Questions;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionToUpdate = await _unitOfWork.QuestionRepository.GetByIdAsync(request.Id,
            include: q => q.Include(o => o.Options));

        if (questionToUpdate != null)
        {
            // Update the main properties
            questionToUpdate.Content = request.Content;
            questionToUpdate.Type = request.Type;
            questionToUpdate.Score = request.Score;
            questionToUpdate.LevelId = request.LevelId.Value;
            questionToUpdate.ImageUrl = request.ImageUrl;
            
            // Delete old options
            var oldOptions = questionToUpdate.Options.ToList();
            foreach (var oldOption in oldOptions)
            {
                _unitOfWork.OptionRepository.Delete(oldOption);
            }

            // Add the new options from the request
            foreach (var optionDto in request.Options)
            {
                questionToUpdate.Options.Add(new Option
                {
                    Content = optionDto.Content,
                    IsCorrect = optionDto.IsCorrect
                });
            }
            
            _unitOfWork.QuestionRepository.Update(questionToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}