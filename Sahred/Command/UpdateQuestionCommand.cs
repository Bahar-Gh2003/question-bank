using Domain;
using MediatR;

namespace Sahred.Command;

public class UpdateQuestionCommand : IRequest
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public string? ImageUrl { get; set; }
    public int Score { get; set; }
    public Guid? LevelId { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
}