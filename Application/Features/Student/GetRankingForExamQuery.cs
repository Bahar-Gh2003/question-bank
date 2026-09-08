using MediatR;
using Shared;

namespace Application.Features.Student;

public class GetRankingForExamQuery : IRequest<List<ExamResultRankDto>>
{
    public Guid ExamId { get; set; }
}