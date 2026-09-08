using MediatR;
using Shared.Student;

namespace Application.Features.Student;

public class SubmitExamCommand : IRequest<ExamResultDto>
{
    public Guid UserId { get; set; }
    public Guid AttemptId { get; set; }
    public Dictionary<Guid, string> ShortAnswerTexts { get; set; } = new();
}