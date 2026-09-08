namespace Sahred.Student;

public class ExamSessionDto
{
    /// <summary>شناسه این جلسه آزمون. کلاینت باید در هر درخواست بعدی آن را بفرستد.</summary>
    public Guid AttemptId { get; set; }

    public string ExamTitle { get; set; }
    public int DurationInMinutes { get; set; }

    /// <summary>
    /// ثانیه‌های باقی‌مانده که سرور محاسبه کرده است.
    /// تایمر کلاینت فقط نمایشی است؛ مرجع نهایی همیشه سرور است.
    /// </summary>
    public int RemainingSeconds { get; set; }

    public List<ExamQuestionDto> Questions { get; set; } = new();
}