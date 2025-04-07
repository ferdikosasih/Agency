using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Agency.Service.Authentication;
using FastEndpoints;
using FluentAssertions;

namespace Agency.Xunit;

[Collection("ApiTestCollection")]
public class LoginTests(ApiTestFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
    [Fact]
    public async Task Login_Success()
    {
        //act
        var accessToken = await GetAccessTokenAsync();
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
    private async Task<string> GetAccessTokenAsync(
        string userId = "agent1"
        ,string  password = "12345")
    {
        var loginRequest = new LoginRequest()
        {
            UserId = userId,
            Password = password
        };
        var loginResponse = await fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(loginRequest);
        loginResponse.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        return loginResponse.Result.AccessToken;
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}