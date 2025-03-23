#region License

/*
 * All content copyright Kaustab Samanta, unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuartzifyDashboard.Helpers;

namespace QuartzifyDashboard.Services;

/// <summary>
/// Service responsible for handling user authentication and JWT generation/validation.
/// </summary>
public class AuthService
{
    private readonly AuthSettings _authSettings;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="authSettings">Injected authentication settings.</param>
    /// <param name="logger">Logger for tracking authentication-related logs.</param>
    public AuthService(IOptions<AuthSettings> authSettings, ILogger<AuthService> logger)
    {
        _authSettings = authSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user based on the provided username and password.
    /// </summary>
    /// <param name="username">The username provided by the user.</param>
    /// <param name="password">The password provided by the user.</param>
    /// <returns>A JWT token if authentication is successful; otherwise, null.</returns>
    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var configuredUsername = _authSettings.Username;
        var configuredPassword = _authSettings.Password;

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

    /// <summary>
    /// Validates a given JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
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
                ValidIssuer = _authSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _authSettings.Audience,
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

    /// <summary>
    /// Generates a JWT token for the authenticated user.
    /// </summary>
    /// <param name="username">The username for whom the token is generated.</param>
    /// <returns>A signed JWT token string.</returns>
    private string GenerateJwtToken(string username)
    {
        var secretKey = GetSecretKey();
        var issuer = _authSettings.Issuer;
        var audience = _authSettings.Audience;

        var tokenExpiryMinutes = 60;
        if (int.TryParse(_authSettings.TokenExpiryMinutes, out var configuredExpiry))
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

    /// <summary>
    /// Retrieves the symmetric security key based on the configured secret.
    /// If no secret is provided, a temporary random key is generated (not recommended for production).
    /// </summary>
    /// <returns>A symmetric security key derived from the configured secret.</returns>
    private SymmetricSecurityKey GetSecretKey()
    {
        var secret = _authSettings.Secret;

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