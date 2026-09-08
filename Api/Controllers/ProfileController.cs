using System.Security.Claims;
using Application.Features.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sahred;
using Sahred.Student;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetUserProfile()
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var query = new GetMyProfileQuery { UserId = userId };
        var profile = await _mediator.Send(query);
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var command = new UpdateProfileCommand
        {
            UserId = userId,
            FullName = dto.FullName,
            Email = dto.Email
        };
        await _mediator.Send(command);
        return NoContent();
    }
    
    [HttpPost("picture")]
    public async Task<IActionResult> UpdateProfilePicture([FromBody] UpdateProfilePictureDto dto)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var command = new UpdateProfilePictureCommand { UserId = userId, ImageUrl = dto.ImageUrl };
        await _mediator.Send(command);
        return NoContent();
    }
}