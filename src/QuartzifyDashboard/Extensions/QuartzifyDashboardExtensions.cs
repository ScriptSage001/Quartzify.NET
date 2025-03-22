using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using QuartzifyDashboard.Helpers;
using QuartzifyDashboard.Middleware;
using QuartzifyDashboard.Services;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods to configure Quartz Dashboard services and middleware.
/// </summary>
public static class QuartzDashboardExtensions
{
    /// <summary>
    /// Adds Quartz Dashboard services and JWT authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object containing authentication settings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddQuartzDashboard(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddJwtBearer(configuration);
        services.ConfigureJsonOptions();
        services.AddConfigurations(configuration);
            
        return services;
    }

    /// <summary>
    /// Registers core services needed by the Quartz Dashboard.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<QuartzService>();
        services.AddHostedService<QuartzService>();
        services.AddSingleton<AuthService>();
    }

    /// <summary>
    /// Configures JWT authentication for the Quartz Dashboard.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration object containing JWT settings.</param>
    private static void AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["QuartzDashboard:Auth:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            jwtSecret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["QuartzDashboard:Auth:Issuer"] ?? "QuartzDashboard",
                    ValidateAudience = true,
                    ValidAudience = configuration["QuartzDashboard:Auth:Audience"] ?? "QuartzDashboardUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
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
    
    /// <summary>
    /// Sets up the Quartz Dashboard middleware and static file handling.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="routePrefix">The route prefix for accessing the dashboard. Defaults to "quartzify".</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseQuartzDashboard(this IApplicationBuilder app, string routePrefix = "quartzify")
    {
        routePrefix = routePrefix.Trim('/');

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapQuartzDashboardEndpoints(routePrefix);
        });

        var assembly = Assembly.GetExecutingAssembly();
        var uiResourcePath = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith("ui.package.zip"));

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(assembly, Path.GetDirectoryName(uiResourcePath)),
            RequestPath = $"/{routePrefix}"
        });

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
}