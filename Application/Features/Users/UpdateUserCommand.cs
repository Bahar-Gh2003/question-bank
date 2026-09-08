using MediatR;
using Shared.Admin;

namespace Application.Features.Users;

public class UpdateUserCommand : AdminUpdateUserDto, IRequest
{
    
}