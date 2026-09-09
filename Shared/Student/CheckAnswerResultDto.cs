namespace Shared.Student;

/// <summary>
/// The server's verdict on a single option.
/// Only the outcome is returned, never which option was correct.
/// </summary>
public class CheckAnswerResultDto
{
    public bool IsCorrect { get; set; }
    public int AttemptsUsed { get; set; }
    public bool IsLocked { get; set; }
    public string Message { get; set; } = "";
}