using Domain;

namespace Shared.Student;

public class ExamQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }

    // ⚠️ CorrectOptionId حذف شد: پاسخ صحیح هرگز نباید به مرورگر دانشجو برود.
    // تشخیص درست/غلط از طریق endpoint سرور (api/student/check-answer) انجام می‌شود.

    /// <summary>تعداد پاسخ‌های ثبت‌شده برای این سوال (برای وقتی دانشجو صفحه را رفرش می‌کند).</summary>
    public int AttemptsUsed { get; set; }

    /// <summary>آیا این سوال قفل شده است؟ (پاسخ درست داده یا ۲ بار اشتباه زده)</summary>
    public bool IsLocked { get; set; }

    /// <summary>گزینه‌ای که دانشجو قبلاً انتخاب کرده بود (برای بازیابی بعد از رفرش).</summary>
    public Guid? SelectedOptionId { get; set; }

    /// <summary>متن تشریحی که دانشجو قبلاً نوشته بود.</summary>
    public string? ShortAnswerText { get; set; }
}