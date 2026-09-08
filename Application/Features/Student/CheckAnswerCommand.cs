using MediatR;
using Sahred.Student;

namespace Application.Features.Student;

public class CheckAnswerCommand : IRequest<CheckAnswerResultDto>
{
    public Guid UserId { get; set; }
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}