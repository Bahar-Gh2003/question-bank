namespace Shared.Student;

public class ExamSessionDto
{
    /// <summary>Identifies this exam session. The client must send it with every follow-up request.</summary>
    public Guid AttemptId { get; set; }

    public string ExamTitle { get; set; }
    public int DurationInMinutes { get; set; }

    /// <summary>
    /// Seconds remaining, as calculated by the server.
    /// The client timer is display only; the server is always authoritative.
    /// </summary>
    public int RemainingSeconds { get; set; }

    public List<ExamQuestionDto> Questions { get; set; } = new();
}