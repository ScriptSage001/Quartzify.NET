namespace QuartzifyDashboard.Models;

public class SchedulerStatus
{
    public string SchedulerName { get; set; }
    public string SchedulerInstanceId { get; set; }
    public bool IsStarted { get; set; }
    public bool IsShutdown { get; set; }
    public bool IsStandbyMode { get; set; }
    public string JobStoreType { get; set; }
    public string ThreadPoolType { get; set; }
    public int ThreadPoolSize { get; set; }
    public string Version { get; set; }
}