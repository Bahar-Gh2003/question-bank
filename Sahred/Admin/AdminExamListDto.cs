namespace Sahred.Admin;

public class AdminExamListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string LevelTitle { get; set; }
    // public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}