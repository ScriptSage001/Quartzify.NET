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
        apiGroup.MapPost("/auth/login", async (LoginRequest request, AuthService authService) =>
        {
            var token = await authService.AuthenticateAsync(request.Username, request.Password);
            return string.IsNullOrEmpty(token) ? 
                Results.Unauthorized() : 
                Results.Ok(new { token });
        });

        // Scheduler endpoints
        var securedApiGroup = endpoints
                                .MapGroup($"{routePrefix}/api")
                                .RequireAuthorization();
        
        securedApiGroup.MapGet("/scheduler/status", async (QuartzService quartzService) =>
        {
            var status = await quartzService.GetSchedulerStatusAsync();
            return Results.Ok(status);
        });

        securedApiGroup.MapGet("/jobs", async (QuartzService quartzService) =>
        {
            var jobs = await quartzService.GetAllJobsAsync();
            return Results.Ok(jobs);
        });

        securedApiGroup.MapGet("/triggers", async (QuartzService quartzService) =>
        {
            var triggers = await quartzService.GetAllTriggersAsync();
            return Results.Ok(triggers);
        });

        securedApiGroup.MapGet("/history", async (QuartzService quartzService, int? count) =>
        {
            var history = await quartzService.GetRecentExecutionHistoryAsync(count ?? 50);
            return Results.Ok(history);
        });

        securedApiGroup.MapPost("/jobs/{jobKey}/pause", async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.PauseJobAsync(jobKey);
            return Results.Ok(new { message = $"Job {jobKey} paused" });
        });

        securedApiGroup.MapPost("/jobs/{jobKey}/resume", async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.ResumeJobAsync(jobKey);
            return Results.Ok(new { message = $"Job {jobKey} resumed" });
        });

        securedApiGroup.MapPost("/jobs/{jobKey}/trigger", async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.TriggerJobAsync(jobKey);
            return Results.Ok(new { message = $"Job {jobKey} triggered" });
        });

        securedApiGroup.MapDelete("/jobs/{jobKey}", async (string jobKey, QuartzService quartzService) =>
        {
            await quartzService.DeleteJobAsync(jobKey);
            return Results.Ok(new { message = $"Job {jobKey} deleted" });
        });

        securedApiGroup.MapPost("/triggers/{triggerKey}/pause", async (string triggerKey, QuartzService quartzService) =>
        {
            await quartzService.PauseTriggerAsync(triggerKey);
            return Results.Ok(new { message = $"Trigger {triggerKey} paused" });
        });

        securedApiGroup.MapPost("/triggers/{triggerKey}/resume", async (string triggerKey, QuartzService quartzService) =>
        {
            await quartzService.ResumeTriggerAsync(triggerKey);
            return Results.Ok(new { message = $"Trigger {triggerKey} resumed" });
        });

        securedApiGroup.MapPost("/scheduler/start", async (QuartzService quartzService) =>
        {
            var result = await quartzService.StartSchedulerAsync();
            return Results.Ok(new { success = result, message = result ? "Scheduler started" : "Scheduler already running" });
        });

        securedApiGroup.MapPost("/scheduler/standby", async (QuartzService quartzService) =>
        {
            var result = await quartzService.StandbySchedulerAsync();
            return Results.Ok(new { success = result, message = result ? "Scheduler in standby mode" : "Scheduler already in standby mode" });
        });

        securedApiGroup.MapPost("/scheduler/shutdown", async (QuartzService quartzService, bool? waitForJobsToComplete) =>
        {
            var result = await quartzService.ShutdownSchedulerAsync(waitForJobsToComplete ?? true);
            return Results.Ok(new { success = result, message = result ? "Scheduler shutdown" : "Scheduler already shutdown" });
        });
    }
}