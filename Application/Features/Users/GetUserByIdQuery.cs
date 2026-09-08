using MediatR;
using Sahred.Admin;

namespace Application.Features.Users;

public class GetUserByIdQuery : IRequest<AdminUpdateUserDto>
{
    public Guid UserId { get; set; }
}