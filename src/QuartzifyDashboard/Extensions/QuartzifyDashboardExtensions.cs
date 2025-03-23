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

using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using QuartzifyDashboard.Helpers;
using QuartzifyDashboard.Middleware;
using QuartzifyDashboard.Services;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods to configure Quartzify Dashboard services and middleware.
/// </summary>
public static class QuartzifyDashboardExtensions
{
    private const string AddJobAndTrigger = nameof(AddJobAndTrigger);
    
    #region DI Extensions

    /// <summary>
    /// Adds Quartz Dashboard services and JWT authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object containing authentication settings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddQuartzifyDashboard(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddCustomAuthentication(configuration);
        services.ConfigureJsonOptions();
        services.AddConfigurations(configuration);
            
        return services;
    }

    /// <summary>
    /// Adds Quartz services and jobs along with Quartzify Dashboard services to the service collection.
    /// Dynamically scans the provided assemblies for job types and registers them with Quartz.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object containing Quartz settings.</param>
    /// <param name="assemblies">The assemblies to scan for Quartz jobs.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddQuartzWithQuartzify(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        services.AddQuartz(q =>
        {
            var jobTypes = (assemblies.Length > 0 ? 
                    assemblies : AppDomain.CurrentDomain.GetAssemblies())
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(type => 
                            typeof(IJob).IsAssignableFrom(type) && 
                            type is { IsInterface: false, IsAbstract: false });

            foreach (var jobType in jobTypes)
            {
                var method = typeof(JobExtensions)
                    .GetMethod(
                        nameof(AddJobAndTrigger), 
                        BindingFlags.Static | BindingFlags.Public)
                    ?.MakeGenericMethod(jobType);
                
                method?.Invoke(null, [q, configuration]);
            }
        });
        
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddQuartzifyDashboard(configuration);
        
        return services;
    }

    /// <summary>
    /// Sets up the Quartz Dashboard middleware and static file handling.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="routePrefix">The route prefix for accessing the dashboard. Defaults to "quartzify".</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseQuartzDashboard(this IApplicationBuilder app, string routePrefix = "quartzify")
    {
        routePrefix = routePrefix.Trim('/');

        app.UseRouting();
        
        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapQuartzDashboardEndpoints(routePrefix);
        });

        // var assembly = Assembly.GetExecutingAssembly();
        // var uiResourcePath = assembly.GetManifestResourceNames()
        //     .Single(str => str.EndsWith("ui.package.zip"));
        //
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     FileProvider = new EmbeddedFileProvider(assembly, Path.GetDirectoryName(uiResourcePath)),
        //     RequestPath = $"/{routePrefix}"
        // });

        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == 404 && 
                context.Request.Path.StartsWithSegments($"/{routePrefix}") &&
                !context.Request.Path.StartsWithSegments($"/{routePrefix}/api"))
            {
                context.Request.Path = $"/{routePrefix}/index.html";
                await next();
            }
        });

        return app;
    }

    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Registers core services needed by the Quartz Dashboard.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    private static void AddServices(this IServiceCollection services)
    {
        services.AddHostedService<QuartzService>();
        services
            .AddSingleton<QuartzService>(serviceProvider =>
            (QuartzService)serviceProvider
                .GetServices<IHostedService>()
                .First(service => service is QuartzService));
        
        services.AddSingleton<AuthService>();
    }

    /// <summary>
    /// Configures JWT authentication for the Quartz Dashboard.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object containing JWT settings.</param>
    private static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Quartzify:Auth:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            jwtSecret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetSection("Quartzify:Auth:Issuer").Value,
                    ValidateAudience = true,
                    ValidAudience = configuration.GetSection("Quartzify:Auth:Audience").Value,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                });
    }

    /// <summary>
    /// Configures JSON serialization options for controllers.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    private static void ConfigureJsonOptions(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
    }

    /// <summary>
    /// Adds configurations from AppSettings.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object.</param>
    private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Quartzify:Auth"));
    }

    #endregion
    
}