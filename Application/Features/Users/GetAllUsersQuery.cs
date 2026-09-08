using MediatR;
using Shared.Admin;

namespace Application.Features.Users;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
    
}