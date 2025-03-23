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