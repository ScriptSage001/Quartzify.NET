namespace QuartzifyDashboard.Models;

public class SchedulerStatus
{
    public required string SchedulerName { get; set; }
    public required string SchedulerInstanceId { get; set; }
    public bool IsStarted { get; set; }
    public bool IsShutdown { get; set; }
    public bool IsStandbyMode { get; set; }
    public required string JobStoreType { get; set; }
    public required string ThreadPoolType { get; set; }
    public int ThreadPoolSize { get; set; }
    public required string Version { get; set; }
}