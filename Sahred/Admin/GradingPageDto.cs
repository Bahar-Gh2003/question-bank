namespace Sahred.Admin;

public class GradingPageDto
{
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; }
    public List<GradingQuestionDto> Questions { get; set; } = new();
}