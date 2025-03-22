using Quartz;

namespace QuartzifyDashboard.Extensions;

public static class JobDataMapExtensions
{
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