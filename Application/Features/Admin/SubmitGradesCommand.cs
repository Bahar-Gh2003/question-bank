using MediatR;
using Shared.Admin;

namespace Application.Features.Admin;

public class SubmitGradesCommand : IRequest
{
    public Guid AttemptId { get; set; }
    public GradingSubmissionDto Dto { get; set; }
}