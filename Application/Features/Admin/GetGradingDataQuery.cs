using MediatR;
using Shared.Admin;

namespace Application.Features.Admin;

public class GetGradingDataQuery : IRequest<List<GradingQuestionDto>>
{
    public Guid AttemptId { get; set; }
}