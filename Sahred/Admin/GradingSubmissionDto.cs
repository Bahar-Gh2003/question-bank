namespace Sahred.Admin;

public class GradingSubmissionDto
{
    public Dictionary<Guid, bool> GradedAnswers { get; set; } = new();
}