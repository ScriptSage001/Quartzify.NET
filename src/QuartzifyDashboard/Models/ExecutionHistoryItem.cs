namespace QuartzifyDashboard.Models;

public class ExecutionHistoryItem
{
    public string Id { get; set; }
    public string JobKey { get; set; }
    public string JobName { get; set; }
    public string JobGroup { get; set; }
    public string TriggerKey { get; set; }
    public string TriggerName { get; set; }
    public string TriggerGroup { get; set; }
    public DateTime FireTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Succeeded { get; set; }
    public string Exception { get; set; }
}