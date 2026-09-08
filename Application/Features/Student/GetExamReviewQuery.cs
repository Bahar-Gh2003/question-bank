using MediatR;
using Shared.Student;

namespace Application.Features.Student;

public class GetExamReviewQuery : IRequest<ExamReviewDto>
{
    public Guid AttemptId { get; set; }
    public Guid UserId { get; set; }
}