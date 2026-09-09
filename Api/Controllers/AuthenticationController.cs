using Application.Features.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        try
        {
            var command = new RegisterUserCommand { Dto = userForRegistration };
            await _mediator.Send(command);
            return StatusCode(201);
        }
        catch (InvalidOperationException ex)
        {
            // Return the error message to the frontend as a Bad Request
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            // For any other unexpected errors
            return StatusCode(500, "یک خطای داخلی در سرور رخ داده است.");
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
    {
        try
        {
            var query = new AuthenticateUserQuery { Dto = userForAuthentication };
            var token = await _mediator.Send(query);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}