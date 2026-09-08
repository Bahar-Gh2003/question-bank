using MediatR;

namespace Application.Features.Profile;

public class UpdateProfileCommand : IRequest
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
}