namespace QuartzifyDashboard.Models;

/// <summary>
/// Represents the result of a paginated query for job execution history.
/// </summary>
public class ExecutionHistoryResult
{
    /// <summary>
    /// The total number of execution history items available.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// A read-only list containing the execution history items for the current query.
    /// </summary>
    public required IReadOnlyList<ExecutionHistoryItem> Items { get; set; }
}