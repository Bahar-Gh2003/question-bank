namespace Shared.Student;

public class ExamResultDto
{
    public int TotalScore { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>
    /// True when the attempt contains short answers that the admin has not graded yet.
    /// TotalScore then covers the multiple-choice questions only.
    /// </summary>
    public bool AwaitingGrading { get; set; }
}