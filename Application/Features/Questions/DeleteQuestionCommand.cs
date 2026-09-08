using MediatR;

namespace Application.Features.Questions;

public class DeleteQuestionCommand : IRequest
{
    public Guid QuestionId { get; set; }
}