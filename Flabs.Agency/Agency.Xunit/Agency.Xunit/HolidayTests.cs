using Agency.Service.Holidays;
using FastEndpoints;
using FluentAssertions;

namespace Agency.Xunit;

[Collection("ApiTestCollection")]
public class HolidayTests(ApiTestFixture fixture) : IAsyncLifetime
{
    private string _accessToken = string.Empty;
    public async Task InitializeAsync()
    {
        _accessToken = await fixture.GetAccessTokenAsync();
        fixture.Client.DefaultRequestHeaders.Authorization = new ("Bearer"
            , _accessToken);
    }
    
    [Fact]
    public async Task ImportHoliday_ShouldSucceed()
    {
        var result = await fixture.Client
            .POSTAsync<HolidayEndpoint, HolidayResponse>();
        result.Response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task ViewHoliday_Success()
    {
        var dateOnlyNow = DateOnly.FromDateTime(DateTime.Now);
        var result = await fixture.Client.GETAsync<ViewEndpoint,HolidayResponse>();
        //assert
        result.Response.EnsureSuccessStatusCode();
        result.Result
            .Should()
            .NotBeNull();
        result.Result.Holidays
            .Should()
            .HaveCountGreaterThanOrEqualTo(1);
    }
    
    public Task DisposeAsync() => Task.CompletedTask;
}