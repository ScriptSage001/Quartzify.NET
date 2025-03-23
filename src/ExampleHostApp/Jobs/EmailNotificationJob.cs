using Quartz;

namespace ExampleHostApp.Jobs;

[DisallowConcurrentExecution]
public class EmailNotificationJob(ILogger<EmailNotificationJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("EmailNotificationJob: Sending notifications at {time}", DateTime.UtcNow);
            
        // Simulate sending emails
        var recipients = new[] { "user1@example.com", "user2@example.com", "user3@example.com" };
        foreach (var recipient in recipients)
        {
            logger.LogDebug("EmailNotificationJob: Sending email to {recipient}", recipient);
            Task.Delay(200).Wait();
        }
            
        logger.LogInformation("EmailNotificationJob: Sent {count} notifications successfully", recipients.Length);
        return Task.CompletedTask;
    }
}