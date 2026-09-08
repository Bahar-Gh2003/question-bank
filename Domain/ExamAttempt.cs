namespace Domain;

public class ExamAttempt : BaseEntity
{
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTime AttemptedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; }

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    public string? StudentAnswersJson { get; set; }
    public Guid? LevelId { get; set; }
    public Level? Level { get; set; }

}