using Domain;
using MediatR;

namespace Shared.Command;

public class CreateQuestionCommand : IRequest<Guid>
{
    public string Content { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }
    public int Score { get; set; }
    public Guid? LevelId { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
}