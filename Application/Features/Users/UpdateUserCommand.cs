using MediatR;
using Sahred.Admin;

namespace Application.Features.Users;

public class UpdateUserCommand : AdminUpdateUserDto, IRequest
{
    
}