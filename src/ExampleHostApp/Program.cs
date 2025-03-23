using ExampleHostApp;
using QuartzifyDashboard.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services
        .AddServices()
        .AddOpenApiDocumentation()
        .AddQuartzWithDashboard(builder.Configuration);
}

{
    var app = builder.Build();

    #region Api Documentation

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Quartzify.NET API Documentation")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithModels(true);
            
        });
    }

    #endregion

    app.UseHttpsRedirection();

    // Use Quartzify Dashboard with default route prefix
    app.UseQuartzDashboard();
    
    app.MapControllers();
    app.Run();  
}