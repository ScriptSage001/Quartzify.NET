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

using Microsoft.Extensions.Logging;
using Quartz;
using QuartzifyDashboard.Models;

namespace QuartzifyDashboard.Services;

/// <summary>
/// Listener that tracks job execution history in the Quartz scheduler.
/// </summary>
public class JobExecutionHistoryListener : IJobListener
{
    private readonly ILogger<JobExecutionHistoryListener> _logger;
    private static readonly List<ExecutionHistoryItem> _history = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionHistoryListener"/> class.
    /// </summary>
    /// <param name="loggerFactory">Factory to create a logger for the listener.</param>
    public JobExecutionHistoryListener(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JobExecutionHistoryListener>();
    }

    /// <summary>
    /// Gets the name of the job listener.
    /// </summary>
    public string Name => "JobExecutionHistoryListener";

    /// <summary>
    /// Invoked before a job is executed.
    /// </summary>
    /// <param name="context">Job execution context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Job about to start: {JobKey}", context.JobDetail.Key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when a job execution is vetoed by a trigger listener.
    /// </summary>
    /// <param name="context">Job execution context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Job execution vetoed: {JobKey}", context.JobDetail.Key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked after a job has been executed, recording its result.
    /// </summary>
    /// <param name="context">Job execution context.</param>
    /// <param name="jobException">Optional exception thrown during job execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Retrieves a subset of the job execution history.
    /// </summary>
    /// <param name="count">The number of history entries to retrieve.</param>
    /// <returns>A list of the most recent execution history items.</returns>
    public static List<ExecutionHistoryItem> GetHistory(int count) => _history.Take(count).ToList();
}