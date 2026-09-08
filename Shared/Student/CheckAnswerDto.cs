namespace Shared.Student;

/// <summary>درخواست بررسی یک پاسخ تستی. کلاینت این را به سرور می‌فرستد.</summary>
public class CheckAnswerDto
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}