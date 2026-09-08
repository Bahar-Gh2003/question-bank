using MediatR;
using Sahred.Student;

namespace Application.Features.Student;

public class GetUserExamHistoryQuery : IRequest<List<ExamAttemptDto>>
{
    public Guid UserId { get; set; }
}