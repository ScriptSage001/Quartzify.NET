using Quartz;

namespace ExampleHostApp.Jobs;

[DisallowConcurrentExecution]
public class SampleJob(ILogger<SampleJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("SampleJob executed at {time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}