using Microsoft.Extensions.Logging;
using Quartz;
using QuartzifyDashboard.Models;

namespace QuartzifyDashboard.Services;

public class JobExecutionHistoryListener : IJobListener
    {
        private readonly ILogger<JobExecutionHistoryListener> _logger;
        private static readonly List<ExecutionHistoryItem> _history = [];

        public JobExecutionHistoryListener(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JobExecutionHistoryListener>();
        }

        public string Name => "JobExecutionHistoryListener";

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Job about to start: {JobKey}", context.JobDetail.Key);
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Job execution vetoed: {JobKey}", context.JobDetail.Key);
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            var item = new ExecutionHistoryItem
            {
                Id = Guid.NewGuid().ToString(),
                JobKey = context.JobDetail.Key.ToString(),
                JobName = context.JobDetail.Key.Name,
                JobGroup = context.JobDetail.Key.Group,
                TriggerKey = context.Trigger.Key.ToString(),
                TriggerName = context.Trigger.Key.Name,
                TriggerGroup = context.Trigger.Key.Group,
                FireTime = context.FireTimeUtc.LocalDateTime,
                Duration = context.JobRunTime,
                Succeeded = jobException == null,
                Exception = jobException?.Message ?? string.Empty,
            };

            _history.Insert(0, item); // Keep newest at the top
            if (_history.Count > 100) _history.RemoveAt(100); // Keep only the latest 100 entries

            _logger.LogInformation("Job executed: {JobKey}, Success: {Success}", context.JobDetail.Key, item.Succeeded);

            return Task.CompletedTask;
        }

        public static List<ExecutionHistoryItem> GetHistory(int count) => _history.Take(count).ToList();
    }