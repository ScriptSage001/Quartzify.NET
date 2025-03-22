using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace QuartzifyDashboard.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var configuredUsername = _configuration["QuartzDashboard:Auth:Username"];
        var configuredPassword = _configuration["QuartzDashboard:Auth:Password"];

        if (string.IsNullOrEmpty(configuredUsername) || string.IsNullOrEmpty(configuredPassword))
        {
            _logger.LogWarning("Authentication credentials not configured properly in appsettings.json");
            return null;
        }

        if (username.Equals(configuredUsername, StringComparison.OrdinalIgnoreCase)
            && password.Equals(configuredPassword, StringComparison.Ordinal))
        {
            _logger.LogInformation("User {username} authenticated successfully", username);
            return GenerateJwtToken(username);
        }

        _logger.LogWarning("Failed authentication attempt for user {username}", username);
        return null;
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = GetSecretKey();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["QuartzDashboard:Auth:Issuer"] ?? "QuartzDashboard",
                ValidateAudience = true,
                ValidAudience = _configuration["QuartzDashboard:Auth:Audience"] ?? "QuartzDashboardUsers",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    private string GenerateJwtToken(string username)
    {
        var secretKey = GetSecretKey();
        var issuer = _configuration["QuartzDashboard:Auth:Issuer"] ?? "QuartzDashboard";
        var audience = _configuration["QuartzDashboard:Auth:Audience"] ?? "QuartzDashboardUsers";

        var tokenExpiryMinutes = 60;
        if (int.TryParse(_configuration["QuartzDashboard:Auth:TokenExpiryMinutes"], out var configuredExpiry))
        {
            tokenExpiryMinutes = configuredExpiry;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            ]),
            Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private SymmetricSecurityKey GetSecretKey()
    {
        var secret = _configuration["QuartzDashboard:Auth:Secret"];

        if (string.IsNullOrEmpty(secret))
        {
            // Generate a random secret if none is configured
            secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _logger.LogWarning(
                "JWT secret not configured. Generated a temporary secret. For production, configure a strong secret in appsettings.json.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}