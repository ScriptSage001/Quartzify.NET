namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates whether the authentication was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The generated authentication token, provided on successful login.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// A message describing the result, such as an error message or success confirmation.
    /// </summary>
    public required string Message { get; set; }
}