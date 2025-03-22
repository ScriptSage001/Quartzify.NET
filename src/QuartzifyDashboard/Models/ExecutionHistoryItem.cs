namespace QuartzifyDashboard.Models;

public class ExecutionHistoryItem
{
    public required string Id { get; set; }
    public required string JobKey { get; set; }
    public required string JobName { get; set; }
    public required string JobGroup { get; set; }
    public required string TriggerKey { get; set; }
    public required string TriggerName { get; set; }
    public required string TriggerGroup { get; set; }
    public DateTime FireTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Succeeded { get; init; }
    public string? Exception { get; set; }
}