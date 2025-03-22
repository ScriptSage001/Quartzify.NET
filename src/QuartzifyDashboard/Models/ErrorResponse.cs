namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents the structure of an error response returned by the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// A short, user-friendly message describing the error.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// A more detailed explanation of the error, typically for debugging purposes.
    /// This may be null if no additional details are available.
    /// </summary>
    public string? DetailedMessage { get; set; }

    /// <summary>
    /// A unique identifier for the request, useful for tracing errors in logs.
    /// </summary>
    public required string TraceId { get; set; }
}