namespace QuartzifyDashboard.Models;

public class JobDetail
{
    public string JobKey { get; set; }
    public string GroupName { get; set; }
    public string JobName { get; set; }
    public string JobType { get; set; }
    public string Description { get; set; }
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public int TriggerCount { get; set; }
    public IDictionary<string, object> JobData { get; set; }
}