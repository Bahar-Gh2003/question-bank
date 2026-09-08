using System.Security.Claims;
using Application.Features.Student;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sahred.Student;

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

    [HttpGet("exams")]
    public async Task<IActionResult> GetAvailableExams()
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var query = new GetAvailableExamsQuery { UserId = userId };
        var exams = await _mediator.Send(query);
        return Ok(exams);
    }

    [HttpGet("take-exam/{examId:guid}")]
    public async Task<IActionResult> GetExamQuestions(Guid examId)
    {
        var query = new GetExamQuestionsQuery { ExamId = examId };
        var questions = await _mediator.Send(query);
        return Ok(questions);
    }

    [HttpPost("submit-exam")]
    public async Task<ActionResult<ExamResultDto>> SubmitExam([FromBody] SubmitExamDto submission)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var command = new SubmitExamCommand
        {
            UserId = userId,
            ExamId = submission.ExamId,
            MultipleChoiceAnswers = submission.MultipleChoiceAnswers,
            ShortAnswerTexts = submission.ShortAnswerTexts
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetExamHistory()
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var query = new GetUserExamHistoryQuery { UserId = userId };
        var history = await _mediator.Send(query);
        return Ok(history);
    }

    // [HttpGet("ranking/{examId:guid}")]
    // [AllowAnonymous]
    // public async Task<IActionResult> GetRankingForExam(Guid examId)
    // {
    //     var ranking = await _mediator.Send(new GetRankingForExamQuery { ExamId = examId });
    //     return Ok(ranking);
    // }
    
    [HttpGet("review/{attemptId:guid}")]
    public async Task<ActionResult<ExamReviewDto>> GetExamReview(Guid attemptId)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var query = new GetExamReviewQuery { AttemptId = attemptId, UserId = userId };
        var reviewData = await _mediator.Send(query);
        return Ok(reviewData);
    }
    
    [HttpGet("attempt-status/{attemptId:guid}")]
    public async Task<ActionResult<AttemptStatusDto>> GetAttemptStatus(Guid attemptId)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var query = new GetAttemptStatusQuery { AttemptId = attemptId, UserId = userId };
        var status = await _mediator.Send(query);
        return Ok(status);
    }
}