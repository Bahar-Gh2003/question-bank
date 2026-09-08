namespace Domain;

public class StudentAnswer : BaseEntity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }

    public Guid? SelectedOptionId { get; set; }  // سوال تستی
    public string? ShortAnswerText { get; set; }    // سوال تشریحی
    public bool? IsCorrect { get; set; }

    /// <summary>تعداد دفعاتی که دانشجو به این سوال پاسخ داده. فقط در سرور تغییر می‌کند.</summary>
    public int AttemptCount { get; set; }
}