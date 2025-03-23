using Quartz;

namespace ExampleHostApp;

public static class QuartzExtensions
{
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