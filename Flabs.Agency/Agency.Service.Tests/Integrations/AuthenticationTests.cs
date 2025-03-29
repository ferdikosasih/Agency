using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Agency.Service.Authentication;
using Agency.Service.Tests.Infrastructure;
using FastEndpoints;
using FluentAssertions;

namespace Agency.Service.Tests.Integrations;

[Collection("ApiTestCollection")]
public class AuthenticationTests(ApiTestFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task Login_Success()
    {
        //act
        var accessToken = await fixture.GetAccessTokenAsync();
        //assert
        accessToken.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        jwt.Should().NotBeNull();
        jwt.Claims.Should().Contain(c => c.Type == "sub" && c.Value == "agent1");
        jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == "Agent 1 Name");
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Agent");
        jwt.Claims.Should().Contain(c => c.Type == "iss" && c.Value == "Agency.Service.Authentication");
        jwt.Claims.Should().Contain(c => c.Type == "aud" && c.Value == "Agency.Service");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized()
    {
        //arrange
        var loginRequest = new LoginRequest()
        {
            UserId = "wronguser",
            Password = "wrongpassword"
        };
        var result = await 
            fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(loginRequest);
        result.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest()
    {
        var loginRequest = new LoginRequest();
        var result = await 
            fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(loginRequest);
        result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}