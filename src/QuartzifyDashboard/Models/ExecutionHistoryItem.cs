namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents a single execution record of a Quartz job.
/// </summary>
public class ExecutionHistoryItem
{
    /// <summary>
    /// A unique identifier for the execution history entry.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The key identifying the job that was executed.
    /// </summary>
    public required string JobKey { get; set; }

    /// <summary>
    /// The name of the job that was executed.
    /// </summary>
    public required string JobName { get; set; }

    /// <summary>
    /// The group to which the job belongs.
    /// </summary>
    public required string JobGroup { get; set; }

    /// <summary>
    /// The key identifying the trigger that fired the job.
    /// </summary>
    public required string TriggerKey { get; set; }

    /// <summary>
    /// The name of the trigger that fired the job.
    /// </summary>
    public required string TriggerName { get; set; }

    /// <summary>
    /// The group to which the trigger belongs.
    /// </summary>
    public required string TriggerGroup { get; set; }

    /// <summary>
    /// The exact date and time the job was fired.
    /// </summary>
    public DateTime FireTime { get; set; }

    /// <summary>
    /// The duration of the job execution.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Indicates whether the job execution was successful.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// An optional field containing the exception message if the job failed.
    /// </summary>
    public string? Exception { get; set; }
}