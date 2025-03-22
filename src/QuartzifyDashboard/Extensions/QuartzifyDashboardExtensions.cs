using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using QuartzifyDashboard.Middleware;
using QuartzifyDashboard.Services;

namespace QuartzifyDashboard.Extensions
{
    public static class QuartzDashboardExtensions
    {
        public static IServiceCollection AddQuartzDashboard(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices();
            services.AddJwtBearer(configuration);
            services.ConfigureJsonOptions();
            
            return services;
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<QuartzService>();
            services.AddHostedService<QuartzService>();
            services.AddSingleton<AuthService>();
        }

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

        private static void ConfigureJsonOptions(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => 
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
        }
        
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
}