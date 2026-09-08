using MediatR;

namespace Application.Features.Profile;

public class UpdateProfilePictureCommand : IRequest
{
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; }
}