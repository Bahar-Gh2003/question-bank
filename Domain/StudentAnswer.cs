namespace Domain;

public class StudentAnswer : BaseEntity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }

    public Guid? SelectedOptionId { get; set; }  // Multiple-choice question
    public string? ShortAnswerText { get; set; }    // Short-answer question
    public bool? IsCorrect { get; set; }

    /// <summary>How many times the student answered this question. Only ever changed server-side.</summary>
    public int AttemptCount { get; set; }
}