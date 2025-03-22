namespace QuartzifyDashboard.Models;

public class AuthResult
{
    public bool Success { get; set; }
    public required string Token { get; set; }
    public required string Message { get; set; }
}