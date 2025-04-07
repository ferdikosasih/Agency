using System.Net;
using Agency.Service.Authentication;
using Agency.Service.Infrastructure;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Agency.Xunit;

public class ApiTestFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; private set ; }
    public ApplicationDbContext DbContext { get; private set; }
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("Agency.Service")
        .WithUsername("postgres")
        .WithPassword("admin")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.ConfigureServices(services =>
                {
                    services.SetupDatabase(_dbContainer);
                });

            });
        Client = _factory.CreateClient();
        DbContext = _factory.Services
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        DbContext.SeedDatabase();
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
        var loginResponse = await Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(loginRequest);
        loginResponse.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        return loginResponse.Result.AccessToken;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DbContext.DisposeAsync();
        Client?.Dispose();
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
}