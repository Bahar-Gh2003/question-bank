namespace Domain;

public class Question : BaseEntity
{
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }
    public int Score { get; set; }
    public Guid? LevelId { get; set; }
    public Level? Level { get; set; }
    public ICollection<Option> Options { get; set; } = new List<Option>();
}

public enum QuestionType
{
    MultipleChoice,
    ShortAnswer
}
