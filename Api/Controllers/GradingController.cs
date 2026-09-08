using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Application.Features.Admin;
using Sahred.Admin;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class GradingController : ControllerBase
{
    private readonly IMediator _mediator;

    public GradingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{attemptId:guid}")]
    public async Task<IActionResult> GetGradingData(Guid attemptId)
    {
        var query = new GetGradingDataQuery { AttemptId = attemptId };
        var data = await _mediator.Send(query);
        return Ok(data);
    }

    [HttpPost("{attemptId:guid}")]
    public async Task<IActionResult> SubmitGrades(Guid attemptId, [FromBody] GradingSubmissionDto dto)
    {
        var command = new SubmitGradesCommand { AttemptId = attemptId, Dto = dto };
        await _mediator.Send(command);
        return NoContent();
    }
    [HttpGet("exam-results/{examId:guid}")]
    public async Task<IActionResult> GetAdminExamResults(Guid examId)
    {
        var query = new GetAdminExamResultsQuery { ExamId = examId };
        var data = await _mediator.Send(query);
        return Ok(data);
    }
}