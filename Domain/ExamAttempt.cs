namespace Domain;

public class ExamAttempt : BaseEntity
{
    public int Score { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>زمان شروع آزمون (UTC) — مبنای محاسبه تایمر در سرور.</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>زمان ثبت نهایی آزمون (UTC).</summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>تا وقتی false باشد این تلاش هنوز باز است و در شمارش تلاش‌ها حساب نمی‌شود.</summary>
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