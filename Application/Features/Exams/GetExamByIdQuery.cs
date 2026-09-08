using MediatR;
using Sahred.Admin;

namespace Application.Features.Exams;

public class GetExamByIdQuery : IRequest<UpdateExamDto>

{
    public Guid ExamId { get; set; }
}