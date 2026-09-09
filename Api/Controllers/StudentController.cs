using System.Security.Claims;
using Application.Features.Student;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Student;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("exams")]
    public async Task<IActionResult> GetAvailableExams()
    {
        var exams = await _mediator.Send(new GetAvailableExamsQuery { UserId = CurrentUserId });
        return Ok(exams);
    }

    /// <summary>
    /// Replaces the old GET take-exam/{id}.
    /// It is a POST because it creates server-side state, and all eligibility checks happen here.
    /// </summary>
    [HttpPost("start-exam/{examId:guid}")]
    public async Task<ActionResult<ExamSessionDto>> StartExam(Guid examId)
    {
        var session = await _mediator.Send(new StartExamCommand
        {
            UserId = CurrentUserId,
            ExamId = examId
        });
        return Ok(session);
    }

    /// <summary>Grades a single multiple-choice answer server-side. Returns only correct/incorrect.</summary>
    [HttpPost("check-answer")]
    public async Task<ActionResult<CheckAnswerResultDto>> CheckAnswer([FromBody] CheckAnswerDto dto)
    {
        var result = await _mediator.Send(new CheckAnswerCommand
        {
            UserId = CurrentUserId,
            AttemptId = dto.AttemptId,
            QuestionId = dto.QuestionId,
            SelectedOptionId = dto.SelectedOptionId
        });
        return Ok(result);
    }

    [HttpPost("submit-exam")]
    public async Task<ActionResult<ExamResultDto>> SubmitExam([FromBody] SubmitExamDto submission)
    {
        var result = await _mediator.Send(new SubmitExamCommand
        {
            UserId = CurrentUserId,
            AttemptId = submission.AttemptId,
            ShortAnswerTexts = submission.ShortAnswerTexts
        });
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetExamHistory()
    {
        var history = await _mediator.Send(new GetUserExamHistoryQuery { UserId = CurrentUserId });
        return Ok(history);
    }

    [HttpGet("review/{attemptId:guid}")]
    public async Task<ActionResult<ExamReviewDto>> GetExamReview(Guid attemptId)
    {
        var reviewData = await _mediator.Send(new GetExamReviewQuery
        {
            AttemptId = attemptId,
            UserId = CurrentUserId
        });
        return Ok(reviewData);
    }

    [HttpGet("attempt-status/{attemptId:guid}")]
    public async Task<ActionResult<AttemptStatusDto>> GetAttemptStatus(Guid attemptId)
    {
        var status = await _mediator.Send(new GetAttemptStatusQuery
        {
            AttemptId = attemptId,
            UserId = CurrentUserId
        });
        return Ok(status);
    }
}