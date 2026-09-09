namespace Shared.Student;

/// <summary>A request to grade one multiple-choice answer, sent by the client.</summary>
public class CheckAnswerDto
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}