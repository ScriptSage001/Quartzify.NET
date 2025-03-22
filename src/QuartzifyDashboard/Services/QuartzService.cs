using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Polly;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzifyDashboard.Models;

namespace QuartzifyDashboard.Services;

public class QuartzService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private IScheduler? _scheduler;
        
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<QuartzService> _logger;

    public QuartzService(ISchedulerFactory schedulerFactory, ILoggerFactory loggerFactory)
    {
        _schedulerFactory = schedulerFactory;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<QuartzService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    private async Task InitializeScheduler(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_schedulerFactory);
            
        try
        {
            _logger.LogInformation("Initializing Quartz Scheduler..."); 
                
            _scheduler = await _schedulerFactory.GetScheduler(cancellationToken)
                         ?? throw new InvalidOperationException("Failed to initialize Quartz scheduler");
                
            _logger.LogInformation("Quartz Scheduler initialized."); 
                
            _logger.LogInformation("Starting Quartz Scheduler...");

            if (_scheduler.ListenerManager.GetJobListeners()
                .All(l => l.Name != nameof(JobExecutionHistoryListener)))
            {
                _scheduler.ListenerManager.AddJobListener(
                    new JobExecutionHistoryListener(_loggerFactory),
                    GroupMatcher<JobKey>.AnyGroup()
                );

                _logger.LogInformation("JobExecutionHistoryListener registered.");
            }

            if (!_scheduler.IsStarted)
            {
                await _scheduler.Start(cancellationToken);
                _logger.LogInformation("Quartz Scheduler started successfully.");
            }
            else
            {
                _logger.LogWarning("Quartz Scheduler was already running.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Quartz Scheduler.");
            throw;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Quartz Service...");

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning("Attempt {retryCount} failed. Retrying in {totalSeconds} seconds. Exception: {exception}", 
                        retryCount, timeSpan.TotalSeconds, exception);
                });

        await retryPolicy.ExecuteAsync(() => InitializeScheduler(cancellationToken));
            
        _logger.LogInformation("Quartz Service started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Stopping Quartz Scheduler...");

            if (_scheduler is { IsShutdown: false })
            {
                await _scheduler.Shutdown(cancellationToken);
                _logger.LogInformation("Quartz Scheduler stopped gracefully.");
            }
            else
            {
                _logger.LogWarning("Quartz Scheduler was already shut down.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop Quartz Scheduler.");
            throw;
        }
    }
        
    public async Task<IReadOnlyList<JobDetail>> GetAllJobsAsync()
    {
        try
        {
            var jobGroups = await _scheduler?.GetJobGroupNames()!;
            var allJobs = new List<JobDetail>();

            foreach (var groupName in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupEquals(groupName);
                var jobKeys = await _scheduler.GetJobKeys(groupMatcher);

                foreach (var jobKey in jobKeys)
                {
                    var detail = await _scheduler.GetJobDetail(jobKey);
                    var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                    var nextFireTime = triggers.Max(t => t.GetNextFireTimeUtc())?.LocalDateTime;
                    var previousFireTime = triggers.Max(t => t.GetPreviousFireTimeUtc())?.LocalDateTime;

                    allJobs.Add(new JobDetail
                    {
                        JobKey = jobKey.ToString(),
                        GroupName = jobKey.Group,
                        JobName = jobKey.Name,
                        JobType = detail.JobType.FullName,
                        Description = detail.Description,
                        NextFireTime = nextFireTime,
                        PreviousFireTime = previousFireTime,
                        TriggerCount = triggers.Count,
                        JobData = detail.JobDataMap.ToDictionary()
                    });
                }
            }

            return allJobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching jobs from Quartz scheduler");
            throw;
        }
    }

    public async Task<IReadOnlyList<TriggerDetail>> GetAllTriggersAsync()
    {
        try
        {
            var triggerGroups = await _scheduler?.GetTriggerGroupNames()!;
            var allTriggers = new List<TriggerDetail>();

            foreach (var groupName in triggerGroups)
            {
                var groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(groupName);
                var triggerKeys = await _scheduler.GetTriggerKeys(groupMatcher);

                foreach (var triggerKey in triggerKeys)
                {
                    var trigger = await _scheduler.GetTrigger(triggerKey);
                    var jobKey = trigger.JobKey;
                    var jobDetail = await _scheduler.GetJobDetail(jobKey);

                    allTriggers.Add(new TriggerDetail
                    {
                        TriggerKey = triggerKey.ToString(),
                        GroupName = triggerKey.Group,
                        TriggerName = triggerKey.Name,
                        JobKey = jobKey.ToString(),
                        JobName = jobKey.Name,
                        Description = trigger.Description,
                        StartTime = trigger.StartTimeUtc.LocalDateTime,
                        EndTime = trigger.EndTimeUtc?.LocalDateTime,
                        NextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime,
                        PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                        TriggerState = await _scheduler.GetTriggerState(triggerKey),
                        TriggerType = trigger.GetType().Name,
                        CronExpression = trigger is ICronTrigger cronTrigger ? cronTrigger.CronExpressionString : null,
                        MisfireInstruction = trigger.MisfireInstruction
                    });
                }
            }

            return allTriggers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching triggers from Quartz scheduler");
            throw;
        }
    }

    public async Task<SchedulerStatus> GetSchedulerStatusAsync()
    {
        try
        {
            var metadata = await _scheduler?.GetMetaData()!;
            return new SchedulerStatus
            {
                SchedulerName = _scheduler.SchedulerName,
                SchedulerInstanceId = _scheduler.SchedulerInstanceId,
                IsStarted = _scheduler.IsStarted,
                IsShutdown = _scheduler.IsShutdown,
                IsStandbyMode = _scheduler.InStandbyMode,
                JobStoreType = metadata.JobStoreType.ToString(),
                ThreadPoolType = metadata.ThreadPoolType.ToString(),
                ThreadPoolSize = metadata.ThreadPoolSize,
                Version = metadata.Version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching scheduler status");
            throw;
        }
    }

    public async Task<ExecutionHistoryResult> GetRecentExecutionHistoryAsync(int count = 50)
    {
        return await Task.FromResult(new ExecutionHistoryResult
        {
            TotalCount = JobExecutionHistoryListener.GetHistory(count).Count,
            Items = JobExecutionHistoryListener.GetHistory(count)
        });
    }

    public async Task PauseJobAsync(string jobKey)
    {
        var key = JobKey.Create(jobKey);
        await _scheduler?.PauseJob(key)!;
        _logger.LogInformation("Paused job: {jobKey} at {Timestamp}", jobKey, DateTime.UtcNow);
    }

    public async Task ResumeJobAsync(string jobKey)
    {
        var key = JobKey.Create(jobKey);
        await _scheduler?.ResumeJob(key)!;
        _logger.LogInformation("Resumed job: {jobKey} at {Timestamp}", jobKey, DateTime.UtcNow);
    }

    public async Task TriggerJobAsync(string jobKey)
    {
        var key = JobKey.Create(jobKey);
        await _scheduler?.TriggerJob(key)!;
        _logger.LogInformation("Manually triggered job: {jobKey} at {Timestamp}", jobKey, DateTime.UtcNow);
    }

    public async Task DeleteJobAsync(string jobKey)
    {
        var key = JobKey.Create(jobKey);
        await _scheduler?.DeleteJob(key)!;
        _logger.LogInformation("Deleted job: {jobKey} at {Timestamp}", jobKey, DateTime.UtcNow);
    }

    public async Task PauseTriggerAsync(string triggerKey)
    {
        var key = new TriggerKey(triggerKey);
        await _scheduler?.PauseTrigger(key)!;
        _logger.LogInformation("Paused trigger: {triggerKey} at {Timestamp}", triggerKey, DateTime.UtcNow);
    }

    public async Task ResumeTriggerAsync(string triggerKey)
    {
        var key = new TriggerKey(triggerKey);
        await _scheduler?.ResumeTrigger(key)!;
        _logger.LogInformation("Resumed trigger: {triggerKey} at {Timestamp}", triggerKey, DateTime.UtcNow);
    }

    public async Task<bool> StartSchedulerAsync()
    {
        if (_scheduler is { IsStarted: true }) 
            return false;
            
        await _scheduler?.Start()!;
        _logger.LogInformation("Scheduler started");
        return true;
    }

    public async Task<bool> StandbySchedulerAsync()
    {
        if (_scheduler is not { IsStarted: true, InStandbyMode: false }) 
            return false;
            
        await _scheduler.Standby();
        _logger.LogInformation("Scheduler in standby mode");
        return true;
    }

    public async Task<bool> ShutdownSchedulerAsync(bool waitForJobsToComplete = true)
    {
        if (_scheduler is { IsShutdown: true }) 
            return false;
            
        await _scheduler?.Shutdown(waitForJobsToComplete)!;
        _logger.LogInformation("Scheduler shutdown (waitForJobsToComplete: {waitForJobsToComplete}) at {Timestamp}", 
            waitForJobsToComplete, DateTime.UtcNow);
        return true;
    }
}