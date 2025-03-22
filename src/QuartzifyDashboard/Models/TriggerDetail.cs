using Quartz;

namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents detailed information about a Quartz trigger.
/// </summary>
public class TriggerDetail
{
    /// <summary>
    /// Gets or sets the unique key for the trigger.
    /// </summary>
    public required string TriggerKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the group the trigger belongs to.
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the name of the trigger.
    /// </summary>
    public required string TriggerName { get; set; }

    /// <summary>
    /// Gets or sets the unique key of the associated job.
    /// </summary>
    public required string JobKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the associated job.
    /// </summary>
    public required string JobName { get; set; }

    /// <summary>
    /// Gets or sets the description of the trigger.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the start time of the trigger.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the optional end time for the trigger.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled fire time of the trigger, if applicable.
    /// </summary>
    public DateTime? NextFireTime { get; set; }

    /// <summary>
    /// Gets or sets the previous fire time of the trigger, if applicable.
    /// </summary>
    public DateTime? PreviousFireTime { get; set; }

    /// <summary>
    /// Gets or sets the current state of the trigger (e.g., Normal, Paused, Complete).
    /// </summary>
    public TriggerState TriggerState { get; set; }

    /// <summary>
    /// Gets or sets the type of the trigger (e.g., Cron, Simple).
    /// </summary>
    public required string TriggerType { get; set; }

    /// <summary>
    /// Gets or sets the cron expression for a cron trigger, if applicable.
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Gets or sets the misfire instruction, which defines how Quartz handles missed executions.
    /// </summary>
    public int MisfireInstruction { get; set; }
}