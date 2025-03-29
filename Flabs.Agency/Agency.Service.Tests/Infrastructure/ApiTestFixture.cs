using System.Net.Http.Json;
using Agency.Service.Authentication;
using Agency.Service.Infrastructure;
using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
#pragma warning disable CS8618, CS9264

namespace Agency.Service.Tests.Infrastructure;

public class ApiTestFixture : AppFixture<Program>
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("Agency.Service")
        .WithUsername("postgres")
        .WithPassword("admin")
        .Build();
    private ApplicationDbContext _dbContext;

    protected override ValueTask SetupAsync()
    {
        Client = CreateClient();
        
        using var scope = Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();
        _dbContext.SeedDatabase();
        
        return ValueTask.CompletedTask;
    }

    protected override async ValueTask PreSetupAsync()
    {
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureDatabase(_dbContainer);
    }

    protected override async ValueTask TearDownAsync()
    {
        Client?.Dispose();
        await _dbContainer.DisposeAsync();
    }

    public async Task<string> GetAccessTokenAsync(
        string userId = "agent1"
        ,string  password = "12345")
    {
        var loginRequest = new LoginRequest()
        {
            UserId = userId,
            Password = password
        };
        var loginResponse = await Client.PostAsJsonAsync("/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return tokenResponse?.AccessToken ?? string.Empty;
    }
}