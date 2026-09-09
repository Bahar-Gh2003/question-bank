namespace Shared.Student;

public class SubmitExamDto
{
    /// <summary>The session created by start-exam.</summary>
    public Guid AttemptId { get; set; }

    // Multiple-choice answers are no longer sent here:
    // they are stored server-side as they happen, via check-answer.

    /// <summary>Key: QuestionId - Value: the short answer text</summary>
    public Dictionary<Guid, string> ShortAnswerTexts { get; set; } = new();
}