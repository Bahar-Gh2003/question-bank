namespace Shared.Admin;

public class UpdateExamDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid LevelId { get; set; }
    // public DateTime? StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public int PassingScore { get; set; }
}