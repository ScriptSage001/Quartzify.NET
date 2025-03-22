namespace QuartzifyDashboard.Models;

public class ExecutionHistoryResult
{
    public int TotalCount { get; set; }
    public required IReadOnlyList<ExecutionHistoryItem> Items { get; set; }
}