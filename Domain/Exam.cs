namespace Domain;

public class Exam : BaseEntity
{
    public string Title { get; set; }
    public int DurationInMinutes { get; set; }
    public int PassingScore { get; set; }

    /// <summary>
    /// How many questions are drawn, at random, from this level's question pool
    /// each time a student starts this exam.
    /// </summary>
    public int QuestionCount { get; set; }

    public Guid LevelId { get; set; }
    public Level? Level { get; set; }
}