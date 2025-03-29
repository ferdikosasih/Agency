using Agency.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Configurations;

public static class DatabaseSetupExtensions
{
    public static IServiceCollection AddDefaultDatabase(
        this IServiceCollection services
        , IConfiguration config)
    {
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp,options) =>
        {
            options.UseNpgsql(config.GetConnectionString("ApplicationDbContext"));
            var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
            options.AddInterceptors(auditInterceptor);
        });
        return services;
    }

    public static IApplicationBuilder DatabaseRefresh(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        return app;
    }
}