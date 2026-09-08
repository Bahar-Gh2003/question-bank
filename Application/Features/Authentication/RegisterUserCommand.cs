using MediatR;
using Shared;

namespace Application.Features.Authentication;

public class RegisterUserCommand : IRequest
{
    public UserForRegistrationDto Dto { get; set; }
}