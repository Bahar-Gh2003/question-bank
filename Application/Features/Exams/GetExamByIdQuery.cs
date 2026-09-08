using MediatR;
using Shared.Admin;

namespace Application.Features.Exams;

public class GetExamByIdQuery : IRequest<UpdateExamDto>

{
    public Guid ExamId { get; set; }
}