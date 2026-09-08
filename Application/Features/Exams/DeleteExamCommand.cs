using MediatR;

namespace Application.Features.Exams;

public class DeleteExamCommand : IRequest
{
    public Guid ExamId { get; set; }
}