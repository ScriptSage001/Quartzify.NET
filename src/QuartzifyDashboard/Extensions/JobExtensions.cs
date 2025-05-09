#region License

/*
 * All content copyright Kaustab Samanta, unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion

using Microsoft.Extensions.Configuration;
using Quartz;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods for Adding Jobs with corresponding Triggers and converting Quartz JobDataMap to a Dictionary.
/// </summary>
public static class JobExtensions
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
    
    /// <summary>
    /// Registers a job of type <typeparamref name="T"/> along with a trigger using the provided configuration.
    /// </summary>
    /// <param name="quartz"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddJobAndTrigger<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration configuration) where T : IJob
    {
        // Get the job type name
        var jobName = typeof(T).Name;

        // Try to find this job in configuration
        var jobConfig = configuration.GetSection("Quartz:Jobs")
            .GetChildren()
            .FirstOrDefault(j => j["Type"]?.Contains(jobName) == true);

        JobKey? jobKey;
        if (jobConfig == null)
        {
            // If not found in configuration, register with default settings
            jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule("0/30 * * * * ?")); // Default: run every 30 seconds
                
            return;
        }

        // Get job info from config
        var jobGroup = jobConfig["Group"] ?? "DefaultGroup";
            
        // Register the job
        jobKey = new JobKey(jobConfig["Name"] ?? jobName, jobGroup);
        quartz.AddJob<T>(opts => opts
            .WithIdentity(jobKey)
            .WithDescription(jobConfig["Description"]));

        // Process triggers
        var triggerSection = jobConfig.GetSection("Triggers").GetChildren();
        foreach (var trigger in triggerSection)
        {
            var triggerName = trigger["Name"] ?? $"{jobName}-trigger";
            var triggerGroup = trigger["Group"] ?? jobGroup;
            var cronExpression = trigger["CronExpression"] ?? "0/30 * * * * ?";

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(triggerName, triggerGroup)
                .WithCronSchedule(cronExpression));
        }
    }
}