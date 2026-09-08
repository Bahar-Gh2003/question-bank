using MediatR;
using Sahred.Student;

namespace Application.Features.Student;

public class GetExamQuestionsQuery : IRequest<ExamSessionDto>
{
    public Guid ExamId { get; set; }
}