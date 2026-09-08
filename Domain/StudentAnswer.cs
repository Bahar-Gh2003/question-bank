namespace Domain;

public class StudentAnswer  : BaseEntity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }

    public Guid? SelectedOptionId { get; set; } // For multiple-choice
    public string? ShortAnswerText { get; set; }   // For short-answer
    public bool? IsCorrect { get; set; } 
}