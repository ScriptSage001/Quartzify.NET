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