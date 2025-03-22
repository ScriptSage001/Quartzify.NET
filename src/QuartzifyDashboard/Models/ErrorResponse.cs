namespace QuartzifyDashboard.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
    public string? DetailedMessage { get; set; }
    public required string TraceId { get; set; }
}