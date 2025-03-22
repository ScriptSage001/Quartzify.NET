namespace QuartzifyDashboard.Models;

public class JobDetail
{
    public required string JobKey { get; set; }
    public required string GroupName { get; set; }
    public required string JobName { get; set; }
    public required string JobType { get; set; }
    public required string Description { get; set; }
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public int TriggerCount { get; set; }
    public required IDictionary<string, object> JobData { get; set; }
}