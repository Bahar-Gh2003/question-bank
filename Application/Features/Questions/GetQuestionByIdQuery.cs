using MediatR;
using Shared.Command;

namespace Application.Features.Questions;

public class GetQuestionByIdQuery : IRequest<UpdateQuestionCommand>
{
    public Guid Id { get; set; }
}