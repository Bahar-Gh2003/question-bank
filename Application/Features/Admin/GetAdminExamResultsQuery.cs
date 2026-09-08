using MediatR;
using Shared.Admin;

namespace Application.Features.Admin;

public class GetAdminExamResultsQuery : IRequest<AdminExamResultsDto>
{
    public Guid ExamId { get; set; }
}