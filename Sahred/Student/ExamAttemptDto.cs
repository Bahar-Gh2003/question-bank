namespace Sahred.Student;

public class ExamAttemptDto
{
    public Guid AttemptId { get; set; }
    public string ExamTitle { get; set; }
    public string LevelTitle { get; set; }
    public DateTime AttemptedAt { get; set; }
    public int AttemptNumber { get; set; }
}