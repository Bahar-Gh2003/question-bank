namespace Shared.Student;

public class ExamReviewDto
{
    public string ExamTitle { get; set; }

    /// <summary>True while any short answer in this attempt is still waiting to be graded.</summary>
    public bool AwaitingGrading { get; set; }

    public List<ExamReviewQuestionDto> Questions { get; set; } = new();
}