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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using QuartzifyDashboard.Models;
using QuartzifyDashboard.Services;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods to define Quartzify dashboard API endpoints.
/// </summary>
public static class QuartzifyDashboardEndpoints
{
    /// <summary>
    /// Maps the Quartz dashboard API endpoints to the given route prefix.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="routePrefix">The base route prefix for the API endpoints.</param>
    public static void MapQuartzDashboardEndpoints(this IEndpointRouteBuilder endpoints, string routePrefix)
    {
        routePrefix = routePrefix.Trim('/');
            
        var apiGroup = endpoints.MapGroup($"{routePrefix}/api");
            
        // Auth endpoints
        apiGroup.MapPost(
            "/auth/login", 
            [EndpointSummary("Authenticate user and return JWT token")]
            [Tags("Authentication")]
            [ProducesResponseType(200)]
            [ProducesResponseType(401)]
            async (LoginRequest request, AuthService authService) =>
        {
            var token = await authService.AuthenticateAsync(request.Username, request.Password);
            return string.IsNullOrEmpty(token) ? 
                Results.Unauthorized() : 
                TypedResults.Ok(new { token });
        });

        // Scheduler endpoints
        var securedApiGroup = endpoints
                                .MapGroup($"{routePrefix}/api")
                                .RequireAuthorization();
        
        securedApiGroup.MapGet(
            "/scheduler/status", 
            [EndpointSummary("Get Scheduler Status")]
            [Tags("Scheduler")]
            [ProducesResponseType(typeof(SchedulerStatus), 200)]
            async (QuartzService quartzService) =>
        {
            var status = await quartzService.GetSchedulerStatusAsync();
            return TypedResults.Ok(status);
        });

        securedApiGroup.MapGet(
            "/jobs", 
            [EndpointSummary("Get all Jobs")]
            [Tags("Jobs")]
            [ProducesResponseType(typeof(IReadOnlyList<JobDetail>), 200)]
            async (QuartzService quartzService) =>
        {
            var jobs = await quartzService.GetAllJobsAsync();
            return TypedResults.Ok(jobs);
        });

        securedApiGroup.MapGet(
            "/triggers", 
            [EndpointSummary("Get all Triggers")]
            [Tags("Triggers")]
            [ProducesResponseType(typeof(IReadOnlyList<TriggerDetail>), 200)]
            async (QuartzService quartzService) =>
        {
            var triggers = await quartzService.GetAllTriggersAsync();
            return TypedResults.Ok(triggers);
        });

        securedApiGroup.MapGet(
            "/jobs/history", 
            [EndpointSummary("Get recent execution history")]
            [Tags("Jobs")]
            [ProducesResponseType(typeof(ExecutionHistoryResult), 200)]
            async (QuartzService quartzService, int? count) =>
        {
            var history = await quartzService.GetRecentExecutionHistoryAsync(count ?? 50);
            return TypedResults.Ok(history);
        });

        securedApiGroup.MapPost(
            "/jobs/{jobKey}/pause", 
            [EndpointSummary("Pause a Job")]
            [Tags("Jobs")]
            [ProducesResponseType(200)]
            async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.PauseJobAsync(jobKey);
            return TypedResults.Ok(new { message = $"Job {jobKey} paused" });
        });

        securedApiGroup.MapPost(
            "/jobs/{jobKey}/resume", 
            [EndpointSummary("Resume a Job")]
            [Tags("Jobs")]
            [ProducesResponseType(200)]
            async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.ResumeJobAsync(jobKey);
            return TypedResults.Ok(new { message = $"Job {jobKey} resumed" });
        });

        securedApiGroup.MapPost(
            "/jobs/{jobKey}/trigger", 
            [EndpointSummary("Trigger a Job")]
            [Tags("Jobs")]
            [ProducesResponseType(200)]
            async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.TriggerJobAsync(jobKey);
            return TypedResults.Ok(new { message = $"Job {jobKey} triggered" });
        });

        securedApiGroup.MapDelete(
            "/jobs/{jobKey}", 
            [EndpointSummary("Delete a Job")]
            [Tags("Jobs")]
            [ProducesResponseType(200)]
            async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.DeleteJobAsync(jobKey);
            return TypedResults.Ok(new { message = $"Job {jobKey} deleted" });
        });

        securedApiGroup.MapPost(
            "/triggers/{triggerKey}/pause", 
            [EndpointSummary("Pause a Trigger")]
            [Tags("Triggers")]
            [ProducesResponseType(200)]
            async (string triggerKey, QuartzService quartzService) =>
        {
            await quartzService.PauseTriggerAsync(triggerKey);
            return TypedResults.Ok(new { message = $"Trigger {triggerKey} paused" });
        });

        securedApiGroup.MapPost(
            "/triggers/{triggerKey}/resume", 
            [EndpointSummary("Resume a Trigger")]
            [Tags("Triggers")]
            [ProducesResponseType(200)]
            async (string triggerKey, QuartzService quartzService) =>
        {
            await quartzService.ResumeTriggerAsync(triggerKey);
            return TypedResults.Ok(new { message = $"Trigger {triggerKey} resumed" });
        });

        securedApiGroup.MapPost(
            "/scheduler/start", 
            [EndpointSummary("Start the Scheduler")]
            [Tags("Scheduler")]
            [ProducesResponseType(200)]
            async (QuartzService quartzService) =>
        {
            var result = await quartzService.StartSchedulerAsync();
            return TypedResults.Ok(new { success = result, message = result ? "Scheduler started" : "Scheduler already running" });
        });

        securedApiGroup.MapPost(
            "/scheduler/standby", 
            [EndpointSummary("Put the Scheduler in standby mode")]
            [Tags("Scheduler")]
            [ProducesResponseType(200)]
            async (QuartzService quartzService) =>
        {
            var result = await quartzService.StandbySchedulerAsync();
            return TypedResults.Ok(new { success = result, message = result ? "Scheduler in standby mode" : "Scheduler already in standby mode" });
        });

        securedApiGroup.MapPost(
            "/scheduler/shutdown", 
            [EndpointSummary("Shutdown the Scheduler")]
            [Tags("Scheduler")]
            [ProducesResponseType(200)]
            async (QuartzService quartzService, bool? waitForJobsToComplete) =>
        {
            var result = await quartzService.ShutdownSchedulerAsync(waitForJobsToComplete ?? true);
            return TypedResults.Ok(new { success = result, message = result ? "Scheduler shutdown" : "Scheduler already shutdown" });
        });
    }
}