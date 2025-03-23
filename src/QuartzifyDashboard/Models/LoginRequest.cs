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