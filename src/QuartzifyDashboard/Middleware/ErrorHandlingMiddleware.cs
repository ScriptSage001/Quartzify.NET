using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuartzifyDashboard.Models;

namespace QuartzifyDashboard.Middleware;

/// <summary>
/// Middleware for centralized error handling.
/// Captures exceptions and converts them into structured JSON responses.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger instance for logging errors.</param>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the HTTP request and captures any unhandled exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions by logging them and returning a structured error response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The caught exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";
        var detailedMessage = exception.Message;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Authentication required.";
                break;
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Invalid request data.";
                break;
        }

        var result = JsonSerializer.Serialize(new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            DetailedMessage = detailedMessage,
            TraceId = context.TraceIdentifier
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(result);
    }
}