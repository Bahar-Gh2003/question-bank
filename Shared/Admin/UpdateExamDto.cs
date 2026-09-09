using System.ComponentModel.DataAnnotations;

namespace Shared.Admin;

public class UpdateExamDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "عنوان آزمون الزامی است.")]
    public string Title { get; set; }

    public Guid LevelId { get; set; }

    [Range(1, 600, ErrorMessage = "مدت آزمون باید بین ۱ تا ۶۰۰ دقیقه باشد.")]
    public int DurationInMinutes { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "نمره قبولی نمی‌تواند منفی باشد.")]
    public int PassingScore { get; set; }

    [Range(1, 200, ErrorMessage = "تعداد سوالات باید بین ۱ تا ۲۰۰ باشد.")]
    public int QuestionCount { get; set; }
}