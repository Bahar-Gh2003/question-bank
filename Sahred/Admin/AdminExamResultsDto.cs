namespace Sahred.Admin;

public class AdminExamResultsDto
{
    public List<ExamResultRankDto> Attempt1Results { get; set; } = new();
    public List<ExamResultRankDto> Attempt2Results { get; set; } = new();
    public List<ExamResultRankDto> Attempt3Results { get; set; } = new();
}