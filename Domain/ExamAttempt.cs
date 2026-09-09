namespace Domain;

public class ExamAttempt : BaseEntity
{
    public int Score { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>When the exam started (UTC) - the basis for the server-side timer.</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>When the exam was finally submitted (UTC).</summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>While false the attempt is still open and does not count towards the attempt limit.</summary>
    public bool IsCompleted { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ExamId { get; set; }
    public Exam Exam { get; set; }

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    public string? StudentAnswersJson { get; set; }
    public Guid? LevelId { get; set; }
    public Level? Level { get; set; }
}