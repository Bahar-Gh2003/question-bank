namespace Domain;

public class Exam : BaseEntity
{
    public string Title { get; set; }
    // public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public int PassingScore { get; set; }
    public Guid LevelId { get; set; }
    public Level? Level { get; set; }
}