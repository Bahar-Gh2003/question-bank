using MediatR;
using Sahred.Admin;

namespace Application.Features.Users;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
    
}