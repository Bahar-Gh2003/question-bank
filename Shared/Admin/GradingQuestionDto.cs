using Domain;

namespace Shared.Admin;

public class GradingQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public string CorrectAnswer { get; set; }
    public string StudentAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    
    public bool? Iscorrect2 { get; set; }
    public int Score { get; set; }
    public string? ImageUrl { get; set; }
}