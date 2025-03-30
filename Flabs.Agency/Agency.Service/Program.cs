using Agency.Service.Configurations;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    builder.Host.UseSerilog();
    builder.Services
        .AddDefaultDatabase(builder.Configuration)
        .AddSecurity(builder.Configuration)
        .AddFastEndpoints()
        .AddApplicationModule()
        .SwaggerDocument(opt =>
        {
            opt.DocumentSettings = settings =>
            {
                settings.Title = "Agency API";
            };
        });
    
    var app = builder.Build();
    app
        .UseSecurity()
        .UseFastEndpoints()
        .UseSwaggerGen();
    app.DatabaseRefresh();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

namespace Agency.Service
{
    public partial class Program
    {
    }
}
