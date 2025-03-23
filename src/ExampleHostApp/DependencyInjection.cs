using ExampleHostApp.Helpers;
using ExampleHostApp.Jobs;
using Microsoft.OpenApi.Models;
using Quartz;
using QuartzifyDashboard.Extensions;

namespace ExampleHostApp;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        return services;
    }
    
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Quartzify.NET API Documentation",
                    Version = "v1",
                    Description = "<h1>Quartzify.NET API v1.0.0.0</h1>\n" +
                                  "<p><strong>Welcome to the Quartzify.NET API documentation.</strong> 🚀</p>\n" +
                                  "<p>Quartzify.NET is a robust job scheduling service built on top of Quartz.NET, designed to streamline job execution and management for .NET applications.</p>\n" +
                                  "<h2>✨ Key Features</h2>\n" +
                                  "<ul>\n" +
                                  "<li><strong>Flexible Scheduling</strong> ⏳: Supports CRON expressions, simple triggers, and custom intervals.</li>\n" +
                                  "<li><strong>Job Management</strong> 🔧: Add, update, pause, resume, and delete jobs on the fly.</li>\n" +
                                  "<li><strong>Trigger Insights</strong> 🔥: Fetch detailed trigger data, including next fire times, misfire instructions, and job associations.</li>\n" +
                                  "<li><strong>Logging & Monitoring</strong> 📈: Track job executions, failures, and performance metrics with integrated logging.</li>\n" +
                                  "<li><strong>Resilient Operations</strong> 💪: Built-in retry policies and error handling for improved reliability.</li>\n" +
                                  "</ul>\n" +
                                  "<h2>🔮 Future Enhancements</h2>\n" +
                                  "<p>We’re constantly evolving! Future versions will include dynamic job priorities, better concurrency handling, and an advanced dashboard.</p>\n" +
                                  "<p>For more details, visit <a href=\"https://kaustabsamanta.web.app/\">Kaustab Samanta's Portfolio</a> or <a href=\"mailto:scriptsage001@gmail.com\">send an email</a>.</p>\n" +
                                  "<p>Licensed under Quartzify.NET License.</p>"
                };
                return Task.CompletedTask;
            });
        });
        
        return services;
    }
    
    public static IServiceCollection AddQuartzWithDashboard(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            q.AddJobAndTrigger<SampleJob>(configuration);
            q.AddJobAndTrigger<DataCleanupJob>(configuration);
            q.AddJobAndTrigger<EmailNotificationJob>(configuration);
        });
        
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddQuartzifyDashboard(configuration);
        
        return services;
    }
}