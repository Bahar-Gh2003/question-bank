using Domain;

namespace Application.Services;

public record ExamEligibility(bool CanStart, string Status, DateTime? NextAvailableAtUtc);

/// <summary>
/// تنها مرجع تصمیم‌گیری درباره اینکه یک دانشجو مجاز به شرکت در آزمون هست یا نه.
/// هم صفحه «لیست آزمون‌ها» و هم endpoint «شروع آزمون» از همین استفاده می‌کنند
/// تا هرگز بین چیزی که به کاربر نشان داده می‌شود و چیزی که سرور اجازه می‌دهد اختلاف پیش نیاید.
/// همه زمان‌ها UTC هستند.
/// </summary>
public static class ExamEligibilityCalculator
{
    public const int MaxAttempts = 3;
    public const int RetryWaitHours = 24;

    public const string StatusPassed          = "قبول";
    public const string StatusReady           = "آماده شروع";
    public const string StatusReadyRetry      = "آماده تلاش مجدد";
    public const string StatusReadyLastChance = "آماده تلاش مجدد (آخرین فرصت)";
    public const string StatusLocked          = "قفل";
    public const string StatusFailed          = "مردود";

    public static ExamEligibility Evaluate(
        Exam exam,
        IEnumerable<ExamAttempt> attemptsForThisExam,
        DateTime utcNow)
    {
        // فقط تلاش‌های تمام‌شده در شمارش حساب می‌شوند
        var completed = attemptsForThisExam
            .Where(a => a.IsCompleted)
            .OrderBy(a => a.AttemptedAt)
            .ToList();

        if (completed.Any(a => a.IsPassed))
            return new ExamEligibility(false, StatusPassed, null);

        switch (completed.Count)
        {
            // تلاش اول: همیشه در دسترس، بدون تاریخ انقضا
            case 0:
                return new ExamEligibility(true, StatusReady, null);

            // تلاش دوم: بلافاصله بعد از تلاش اول، بدون تاریخ انقضا
            case 1:
                return new ExamEligibility(true, StatusReadyRetry, null);

            // تلاش سوم: فقط بعد از ۲۴ ساعت، و فقط به اندازه مدت آزمون فرصت دارد
            case 2:
                var availableAt = completed[1].AttemptedAt.AddHours(RetryWaitHours);

                if (utcNow < availableAt)
                    return new ExamEligibility(false, StatusLocked, availableAt);

                var deadline = availableAt.AddMinutes(exam.DurationInMinutes);
                return utcNow <= deadline
                    ? new ExamEligibility(true, StatusReadyLastChance, availableAt)
                    : new ExamEligibility(false, StatusFailed, null);

            // سه تلاش انجام شده و قبول نشده
            default:
                return new ExamEligibility(false, StatusFailed, null);
        }
    }
}