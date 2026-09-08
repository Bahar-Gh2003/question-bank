namespace Shared.Admin;

public class GradingSubmissionDto
{
    public Dictionary<Guid, bool> GradedAnswers { get; set; } = new();
}