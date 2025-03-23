using Quartz;

namespace ExampleHostApp.Jobs;

[DisallowConcurrentExecution]
public class DataCleanupJob(ILogger<DataCleanupJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("DataCleanupJob: Cleaning up old data at {time}", DateTime.UtcNow);
            
        // Simulate cleanup work
        Task.Delay(1500).Wait();
            
        logger.LogInformation("DataCleanupJob: Cleanup completed successfully");
        return Task.CompletedTask;
    }
}