namespace Sahred.Student;

public class SubmitExamDto
{
    public Guid ExamId { get; set; }
    public Dictionary<Guid, Guid?> MultipleChoiceAnswers { get; set; } = new();

    // Key: QuestionId, Value: The typed text for short-answer
    public Dictionary<Guid, string> ShortAnswerTexts { get; set; } = new();
}