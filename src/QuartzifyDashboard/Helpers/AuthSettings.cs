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

namespace QuartzifyDashboard.Helpers;

/// <summary>
/// Represents authentication-related configuration settings for the QuartzifyDashboard.
/// </summary>
public class AuthSettings
{
    /// <summary>
    /// The username required for authentication.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// The password associated with the username for authentication.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// The issuer of the JWT token, typically the name of the application or API.
    /// </summary>
    public string Issuer { get; init; } = "QuartzDashboard";

    /// <summary>
    /// The intended audience of the JWT token, usually the group of users allowed access.
    /// </summary>
    public string Audience { get; init; } = "QuartzDashboardUsers";

    /// <summary>
    /// The secret key used to sign and validate the JWT token.
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// The duration (in minutes) for which the JWT token remains valid.
    /// </summary>
    public string TokenExpiryMinutes { get; set; } = "60";
}