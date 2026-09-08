using Application.Features.Levels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LevelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LevelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetLevels()
    {
        var levels = await _mediator.Send(new GetLevelsQuery());
        return Ok(levels);
    }
}