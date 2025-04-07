using System.Net;
using Agency.Service.Appointments;
using FastEndpoints;
using FluentAssertions;

namespace Agency.Xunit;

[Collection("ApiTestCollection")]
public class AppointmentTests(ApiTestFixture fixture) : IAsyncLifetime
{
    private string _accessToken = string.Empty;
    public async Task InitializeAsync()
    {
        _accessToken = await fixture.GetAccessTokenAsync();
        fixture.Client.DefaultRequestHeaders.Authorization = new ("Bearer"
            , _accessToken);
    }

    [Fact]
    public async Task BookingAppoinment_ShouldSucceed()
    {
        var request = new AppointmentRequest()
        {
            CustomerName = "John Doe",
            Location = "London",
            ScheduleDatetime = DateTimeOffset.Now.AddDays(2),
        };
        var result = await fixture.Client
            .POSTAsync<BookingEndpoint, AppointmentRequest, AppointmentResponse>(request);
        result.Response.EnsureSuccessStatusCode();
        result.Result.Token
            .Should()
            .NotBeEmpty()
            .And
            .HaveLength(26);
    }

    [Fact]
    public async Task BookingAppoinment_ShouldValidateInput()
    {
        var errorResponse = new ErrorResponse
        {
            StatusCode = 400,
            Message = "One or more errors occurred!",
            Errors = new Dictionary<string, List<string>>
            {
                { "customerName", ["Customer name is required."] },
                { "location", ["Address is required."] },
                { "scheduleDatetime", ["Schedule datetime is required.", "Appointment must be at least 15 minutes in the future."] }
            }
        };
        
        var result = await fixture.Client
            .POSTAsync<BookingEndpoint, AppointmentRequest, ErrorResponse>(new());
        result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Result.Should().BeEquivalentTo(errorResponse);
    }

    [Fact]
    public async Task BookingAppointment_ShouldPreventHolidays()
    {
        //arrange
        var errorResponse = new ErrorResponse
        {
            StatusCode = 400,
            Message = "One or more errors occurred!",
            Errors = new Dictionary<string, List<string>>
            {
                { "scheduleDatetime", ["Appointment cannot be in holiday calendar"] }
            }
        };
        
        var request = new AppointmentRequest()
        {
            CustomerName = "John Doe",
            Location = "London",
            ScheduleDatetime = DateTimeOffset.Now.AddMinutes(16),
        };
        //act
        var result = await fixture.Client
            .POSTAsync<BookingEndpoint, AppointmentRequest, ErrorResponse>(request);
        //assert
        result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Result.Should().BeEquivalentTo(errorResponse);
    }

    [Fact]
    public async Task BookingAppointment_ShouldPreventUnder15Minutes()
    {
        //arrange
        var errorResponse = new ErrorResponse
        {
            StatusCode = 400,
            Message = "One or more errors occurred!",
            Errors = new Dictionary<string, List<string>>
            {
                { "scheduleDatetime", ["Appointment must be at least 15 minutes in the future."] }
            }
        };
        
        var request = new AppointmentRequest()
        {
            CustomerName = "John Doe",
            Location = "London",
            ScheduleDatetime = DateTimeOffset.Now,
        };
        //act
        var result = await fixture.Client
            .POSTAsync<BookingEndpoint, AppointmentRequest, ErrorResponse>(request);
        //assert
        result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Result.Should().BeEquivalentTo(errorResponse);
    }

    [Fact]
    public async Task ViewAppointment_Success()
    {
        var result = await fixture.Client.GETAsync<ViewEndpoint,List<ViewAppointmentResponse>>();
        result.Response.EnsureSuccessStatusCode();
        result.Result.Should().HaveCountGreaterThanOrEqualTo(1);
        result.Result[0]
            .Appointments
            .Should()
            .HaveCountGreaterThanOrEqualTo(5)
            .And
            .AllSatisfy(a =>
            {
                a.Location.Should().NotBeNullOrEmpty();
                a.Token.Should().NotBeNullOrEmpty();
                a.ScheduleDatetime.Should().BeAfter(DateTimeOffset.Now.Date);
                a.CustomerName.Should().NotBeNullOrEmpty();
            });
    }

    public Task DisposeAsync() => Task.CompletedTask;
}