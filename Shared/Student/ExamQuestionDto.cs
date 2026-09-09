using Domain;

namespace Shared.Student;

public class ExamQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }

    // CorrectOptionId was removed: the correct answer must never reach the student's browser.
    // Grading happens through the server endpoint (api/student/check-answer).

    /// <summary>How many answers were recorded for this question (used when the page is refreshed).</summary>
    public int AttemptsUsed { get; set; }

    /// <summary>Is this question locked? (answered correctly, or missed twice)</summary>
    public bool IsLocked { get; set; }

    /// <summary>The option the student previously selected, restored after a refresh.</summary>
    public Guid? SelectedOptionId { get; set; }

    /// <summary>The short answer the student previously wrote.</summary>
    public string? ShortAnswerText { get; set; }
}