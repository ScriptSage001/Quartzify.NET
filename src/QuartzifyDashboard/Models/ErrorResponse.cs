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