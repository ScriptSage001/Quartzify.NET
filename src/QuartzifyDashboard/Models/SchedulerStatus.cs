namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents the current status and configuration details of the Quartz scheduler.
/// </summary>
public class SchedulerStatus
{
    /// <summary>
    /// Gets or sets the name of the scheduler instance.
    /// </summary>
    public required string SchedulerName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the scheduler instance.
    /// </summary>
    public required string SchedulerInstanceId { get; set; }

    /// <summary>
    /// Indicates whether the scheduler is currently running.
    /// </summary>
    public bool IsStarted { get; set; }

    /// <summary>
    /// Indicates whether the scheduler has been shut down.
    /// </summary>
    public bool IsShutdown { get; set; }

    /// <summary>
    /// Indicates whether the scheduler is in standby mode (paused).
    /// </summary>
    public bool IsStandbyMode { get; set; }

    /// <summary>
    /// Gets or sets the type of job store being used by the scheduler (e.g., RAMJobStore, JDBCJobStore).
    /// </summary>
    public required string JobStoreType { get; set; }

    /// <summary>
    /// Gets or sets the type of thread pool being used by the scheduler.
    /// </summary>
    public required string ThreadPoolType { get; set; }

    /// <summary>
    /// Gets or sets the size of the thread pool.
    /// </summary>
    public int ThreadPoolSize { get; set; }

    /// <summary>
    /// Gets or sets the version of the Quartz scheduler.
    /// </summary>
    public required string Version { get; set; }
}