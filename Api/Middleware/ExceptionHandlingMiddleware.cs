using System.Net;
using System.Text.Json;
using Application.Common;

namespace Api.Middleware;

/// <summary>
/// Maps Application-layer exceptions to the correct HTTP status codes,
/// so controllers no longer need try/catch and clients get a meaningful message.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                NotFoundException      => (HttpStatusCode.NotFound,   ex.Message),
                ForbiddenException     => (HttpStatusCode.Forbidden,  ex.Message),
                BusinessRuleException  => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "خطای داخلی سرور رخ داده است.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }
    }
}