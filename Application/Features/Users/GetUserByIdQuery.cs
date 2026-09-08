using MediatR;
using Shared.Admin;

namespace Application.Features.Users;

public class GetUserByIdQuery : IRequest<AdminUpdateUserDto>
{
    public Guid UserId { get; set; }
}