namespace Shared.Admin;

public class AdminExamListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string LevelTitle { get; set; }
    public int DurationInMinutes { get; set; }
    public int QuestionCount { get; set; }

    /// <summary>How many questions exist in this exam's level, so the admin can spot a shortage.</summary>
    public int AvailableQuestionCount { get; set; }
}