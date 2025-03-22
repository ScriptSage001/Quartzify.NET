using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuartzifyDashboard.Models;

namespace QuartzifyDashboard.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

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