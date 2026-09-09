namespace Shared.Student;

public class AttemptStatusDto
{
    public int StudentScore { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>
    /// True while short answers in this attempt are still waiting for the admin.
    /// StudentScore and IsPassed are provisional until this becomes false.
    /// </summary>
    public bool AwaitingGrading { get; set; }

    public int TotalExamScore { get; set; }
    public int Rank { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public int TotalParticipants { get; set; }
    public int PassingScore { get; set; }
    public Dictionary<int, int> ScoreDistribution { get; set; } = new();
}