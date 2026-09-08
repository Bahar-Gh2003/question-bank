using Application.Features.Questions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Admin;
using Shared.Command;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionDto>>> GetAllQuestions()
    {
        var questions = await _mediator.Send(new GetAllQuestionsQuery());
        return Ok(questions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UpdateQuestionCommand>> GetQuestionById(Guid id)
    {
        var question = await _mediator.Send(new GetQuestionByIdQuery { Id = id });
        return Ok(question);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateQuestion([FromBody] CreateQuestionCommand command)
    {
        var questionId = await _mediator.Send(command);
        return Ok(questionId);
    }
        
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        try
        {
            var command = new DeleteQuestionCommand { QuestionId = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // پیام خطا را به عنوان یک Bad Request به فرانت‌اند ارسال می‌کنیم
            return BadRequest(ex.Message);
        }
    }
}