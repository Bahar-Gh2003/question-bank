using Application.Features.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateExam([FromBody] CreateExamCommand command)
    {
        var examId = await _mediator.Send(command);
        return Ok(examId);
    }

    [HttpGet]

    public async Task<IActionResult> GetAllExams()
    {
        var exams = await _mediator.Send(new GetAllExamsQuery());
        return Ok(exams);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteExam(Guid id)
    {
        try
        {
            var command = new DeleteExamCommand { ExamId = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // Return the error message to the frontend as a Bad Request
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExamById(Guid id)
    {
        var exam = await _mediator.Send(new GetExamByIdQuery { ExamId = id });
        return Ok(exam);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateExam(Guid id, [FromBody] UpdateExamCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();

    }
}