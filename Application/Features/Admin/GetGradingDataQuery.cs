using MediatR;
using Sahred.Admin;

namespace Application.Features.Admin;

public class GetGradingDataQuery : IRequest<List<GradingQuestionDto>>
{
    public Guid AttemptId { get; set; }
}