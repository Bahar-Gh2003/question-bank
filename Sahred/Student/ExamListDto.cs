namespace Sahred.Student;

public class ExamListDto
{
    public Guid ExamId { get; set; }
    public string Title { get; set; }
    public int DurationInMinutes { get; set; }
    public string LevelTitle { get; set; }
    // public DateTime StartTime { get; set; }
    public int AttemptsMade { get; set; }
    public string Status { get; set; } 
    public DateTime? NextAttemptAvailableAt { get; set; }
}