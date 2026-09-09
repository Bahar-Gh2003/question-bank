using Domain;

namespace Application.Services;

public record ExamEligibility(bool CanStart, string Status, DateTime? NextAvailableAtUtc);

/// <summary>
/// The single source of truth for whether a student may take an exam.
/// Both the exam list page and the start-exam endpoint use this,
/// so what the user is shown can never disagree with what the server allows.
/// All times are UTC.
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
    public const string StatusAwaitingGrading = "در انتظار تصحیح";

    public static ExamEligibility Evaluate(
        Exam exam,
        IEnumerable<ExamAttempt> attemptsForThisExam,
        DateTime utcNow)
    {
        // Only completed attempts count towards the limit
        var completed = attemptsForThisExam
            .Where(a => a.IsCompleted)
            .OrderBy(a => a.AttemptedAt)
            .ToList();

        // An attempt still waiting on the admin blocks everything else:
        // the student may yet turn out to have passed it.
        if (completed.Any(a => !a.IsGraded))
            return new ExamEligibility(false, StatusAwaitingGrading, null);

        if (completed.Any(a => a.IsPassed))
            return new ExamEligibility(false, StatusPassed, null);

        switch (completed.Count)
        {
            // First attempt: always available, never expires
            case 0:
                return new ExamEligibility(true, StatusReady, null);

            // Second attempt: available immediately after the first, never expires
            case 1:
                return new ExamEligibility(true, StatusReadyRetry, null);

            // Third attempt: only after 24 hours, and only for the exam's duration
            case 2:
                var availableAt = completed[1].AttemptedAt.AddHours(RetryWaitHours);

                if (utcNow < availableAt)
                    return new ExamEligibility(false, StatusLocked, availableAt);

                var deadline = availableAt.AddMinutes(exam.DurationInMinutes);
                return utcNow <= deadline
                    ? new ExamEligibility(true, StatusReadyLastChance, availableAt)
                    : new ExamEligibility(false, StatusFailed, null);

            // Three attempts made without passing
            default:
                return new ExamEligibility(false, StatusFailed, null);
        }
    }
}