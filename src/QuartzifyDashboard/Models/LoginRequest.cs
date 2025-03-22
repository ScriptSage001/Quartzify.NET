namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents the data required for a user to log in.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The username of the user attempting to log in.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// The password associated with the user's account.
    /// </summary>
    public required string Password { get; set; }
}