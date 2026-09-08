using MediatR;
using Sahred.Student;

namespace Application.Features.Student;

public class SubmitExamCommand : IRequest<ExamResultDto>
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public Dictionary<Guid, Guid?> MultipleChoiceAnswers { get; set; } = new();
    public Dictionary<Guid, string> ShortAnswerTexts { get; set; } = new();
}