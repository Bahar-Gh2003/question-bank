using Domain;

namespace Sahred.Student;

public class ExamQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }
    // public string CorrectAnswer { get; set; }
    public Guid CorrectOptionId { get; set; }
}