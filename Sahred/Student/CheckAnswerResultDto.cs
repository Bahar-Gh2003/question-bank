namespace Sahred.Student;

/// <summary>
/// پاسخ سرور به بررسی یک گزینه.
/// فقط نتیجه برگردانده می‌شود، نه اینکه گزینه درست کدام است.
/// </summary>
public class CheckAnswerResultDto
{
    public bool IsCorrect { get; set; }
    public int AttemptsUsed { get; set; }
    public bool IsLocked { get; set; }
    public string Message { get; set; } = "";
}