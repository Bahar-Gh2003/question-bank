using MediatR;
using Shared.Student;

namespace Application.Features.Student;

public class GetAttemptStatusQuery : IRequest<AttemptStatusDto>
{
    public Guid AttemptId { get; set; }
    public Guid UserId { get; set; }
}