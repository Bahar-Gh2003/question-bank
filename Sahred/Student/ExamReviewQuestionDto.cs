using Domain;

namespace Sahred.Student;

public class ExamReviewQuestionDto
{
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public QuestionType Type { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public string StudentAnswer { get; set; }
    public string CorrectAnswer { get; set; }
    public bool IsStudentAnswerCorrect { get; set; }
    public int Score { get; set; }
    public Guid? SelectedOptionId { get; set; }
    
}