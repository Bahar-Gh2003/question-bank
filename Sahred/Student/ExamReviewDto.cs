namespace Sahred.Student;

public class ExamReviewDto
{
    public string ExamTitle { get; set; }
    public List<ExamReviewQuestionDto> Questions { get; set; } = new();
}