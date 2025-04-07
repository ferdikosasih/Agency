using Agency.Service.Entities;
using Agency.Service.Infrastructure;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Agency.Xunit;

public static class DatabaseExtensions
{
    public static IServiceCollection SetupDatabase(
        this IServiceCollection services,
        PostgreSqlContainer dbContainer)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        if (descriptor != null)
            services.Remove(descriptor);

        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp,options) =>
        {
            options.UseNpgsql(dbContainer.GetConnectionString());
            var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
            options.AddInterceptors(auditInterceptor);
        });
        
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        
        return services;
    }
    public static void SeedDatabase(this ApplicationDbContext dbContext)
    {
        if (!dbContext.Appointments.Any())
        {
            var agentName = "Agent 1 Name";
            var appointments = new Faker<Appointment>()
                .RuleFor(p => p.AgentName, _ => agentName)
                .RuleFor(p => p.CustomerName, f => f.Person.FullName)
                .RuleFor(p => p.Location, f => f.Address.StreetAddress())
                .RuleFor(p => p.ScheduleDatetime, _ => DateTimeOffset.UtcNow.AddMinutes(30))
                .RuleFor(p => p.Token, f => f.Random.AlphaNumeric(26))
                .Generate(5);

            dbContext.Appointments.AddRange(appointments);
        }

        if (!dbContext.Holidays.Any())
        {
            var holidays = new Faker<Holiday>()
                .RuleFor(p=> p.Date, _=> DateOnly.FromDateTime(DateTime.Now))
                .RuleFor(p=> p.Name, f => f.Person.FullName)
                .RuleFor(p=> p.Country, f => f.Address.Country())
                .RuleFor(p=> p.Public, _=>true)
                .RuleFor(p=> p.Uuid, _=>Guid.NewGuid().ToString())
                .Generate(1);
            holidays.Add(
                new Holiday()
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    Name = "Holiday 1",
                    Country = "ID",
                    Public = true,
                    Uuid = Guid.NewGuid().ToString(),
                });
            dbContext.Holidays.AddRange(holidays);
        }

        dbContext.SaveChanges();
    }
}