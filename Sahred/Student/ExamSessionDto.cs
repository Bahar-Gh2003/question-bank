namespace Sahred.Student;

public class ExamSessionDto
{
    public string ExamTitle { get; set; }
    public int DurationInMinutes { get; set; }
    public List<ExamQuestionDto> Questions { get; set; } = new();
    public DateTime StartTime { get; set; }
}