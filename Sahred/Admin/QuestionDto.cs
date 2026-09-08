namespace Sahred.Admin;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Score { get; set; }
    public string LevelTitle { get; set; }
    public string? ImageUrl { get; set; }
}