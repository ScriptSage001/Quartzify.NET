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
/// Represents the details of a scheduled job within Quartz.
/// </summary>
public class JobDetail
{
    /// <summary>
    /// The unique key that identifies the job.
    /// </summary>
    public required string JobKey { get; set; }

    /// <summary>
    /// The group to which the job belongs.
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    /// The name of the job.
    /// </summary>
    public required string JobName { get; set; }

    /// <summary>
    /// The type of the job (usually the fully qualified class name).
    /// </summary>
    public required string JobType { get; set; }

    /// <summary>
    /// A description providing additional details about the job.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// The next scheduled time when the job will fire, if applicable.
    /// </summary>
    public DateTime? NextFireTime { get; set; }

    /// <summary>
    /// The last time the job was executed, if applicable.
    /// </summary>
    public DateTime? PreviousFireTime { get; set; }

    /// <summary>
    /// The number of times the job's trigger has fired.
    /// </summary>
    public int TriggerCount { get; set; }

    /// <summary>
    /// A collection of key-value pairs representing additional job data.
    /// </summary>
    public required IDictionary<string, object> JobData { get; set; }
}