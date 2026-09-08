namespace Sahred.Admin;

public class CreateExamDto
{
    public string Title { get; set; } = string.Empty;
    public Guid? LevelId { get; set; }
    // public DateTime? StartTime { get; set; } = DateTime.Now;
    public int DurationInMinutes { get; set; }
    public int PassingScore { get; set; }
}