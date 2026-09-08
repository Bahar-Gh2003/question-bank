using MediatR;
using Shared;

namespace Application.Features.Authentication;

public class AuthenticateUserQuery : IRequest<string>
{
    public UserForAuthenticationDto Dto { get; set; }
}