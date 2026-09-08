namespace Sahred.Student;

public class SubmitExamDto
{
    /// <summary>شناسه جلسه آزمونی که با start-exam ساخته شده.</summary>
    public Guid AttemptId { get; set; }

    // پاسخ‌های تستی دیگر اینجا فرستاده نمی‌شوند:
    // آنها لحظه‌به‌لحظه از طریق check-answer در سرور ذخیره شده‌اند.

    /// <summary>کلید: QuestionId — مقدار: متن پاسخ تشریحی</summary>
    public Dictionary<Guid, string> ShortAnswerTexts { get; set; } = new();
}