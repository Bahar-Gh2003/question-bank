using MediatR;
using Sahred;

namespace Application.Features.Authentication;

public class RegisterUserCommand : IRequest
{
    public UserForRegistrationDto Dto { get; set; }
}