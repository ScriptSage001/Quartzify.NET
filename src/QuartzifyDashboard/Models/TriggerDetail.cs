using Quartz;

namespace QuartzifyDashboard.Models;

public class TriggerDetail
{
    public string TriggerKey { get; set; }
    public string GroupName { get; set; }
    public string TriggerName { get; set; }
    public string JobKey { get; set; }
    public string JobName { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public TriggerState TriggerState { get; set; }
    public string TriggerType { get; set; }
    public string CronExpression { get; set; }
    public int MisfireInstruction { get; set; }
}