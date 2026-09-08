using MediatR;
using Shared.Student;

namespace Application.Features.Profile;

public class GetMyProfileQuery : IRequest<ProfileDto>
{
    public Guid UserId { get; set; }
}