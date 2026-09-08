using MediatR;

namespace Application.Features.Users;

public class DeleteUserCommand : IRequest
{
    public Guid UserId { get; set; }
}