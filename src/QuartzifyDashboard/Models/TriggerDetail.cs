using Quartz;

namespace QuartzifyDashboard.Models;

public class TriggerDetail
{
    public required string TriggerKey { get; set; }
    public required string GroupName { get; set; }
    public required string TriggerName { get; set; }
    public required string JobKey { get; set; }
    public required string JobName { get; set; }
    public required string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public TriggerState TriggerState { get; set; }
    public required string TriggerType { get; set; }
    public string? CronExpression { get; set; }
    public int MisfireInstruction { get; set; }
}