namespace Sahred;

public class ExamResultRankDto
{
    public Guid AttemptId { get; set; }
    public int Rank { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public string Username { get; set; }
}