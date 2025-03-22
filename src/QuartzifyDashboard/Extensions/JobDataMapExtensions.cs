using Quartz;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods for converting Quartz JobDataMap to a Dictionary.
/// </summary>
public static class JobDataMapExtensions
{
    /// <summary>
    /// Converts a Quartz JobDataMap to a Dictionary with string keys and object values.
    /// </summary>
    /// <param name="map">The JobDataMap to convert.</param>
    /// <returns>A dictionary containing all key-value pairs from the JobDataMap.</returns>
    public static Dictionary<string, object> ToDictionary(this JobDataMap map)
    {
        var result = new Dictionary<string, object>();
        foreach (var key in map.Keys)
        {
            result[key] = map[key];
        }
        return result;
    }
}