using MediatR;
using Sahred;

namespace Application.Features.Authentication;

public class AuthenticateUserQuery : IRequest<string>
{
    public UserForAuthenticationDto Dto { get; set; }
}