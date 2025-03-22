namespace QuartzifyDashboard.Models;

public class ExecutionHistoryResult
{
    public int TotalCount { get; set; }
    public IReadOnlyList<ExecutionHistoryItem> Items { get; set; }
}