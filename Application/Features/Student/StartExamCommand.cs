using MediatR;
using Shared.Student;

namespace Application.Features.Student;

public class StartExamCommand : IRequest<ExamSessionDto>
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
}