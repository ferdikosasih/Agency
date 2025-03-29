using Agency.Service.Entities;
using Agency.Service.Holidays;
using Agency.Service.Infrastructure;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Agency.Service.Appointments;

public interface IBookingService
{
    Task<Result<string>> MakeAppointmentAsync(AppointmentRequest request);
}

public class BookingService(
    ApplicationDbContext dbContext
    , [FromServices] IHolidayService holidayService
    , IHttpContextAccessor httpContextAccessor) : IBookingService
{
    
    public async Task<Result<string>> MakeAppointmentAsync(AppointmentRequest request)
    {
        if (await holidayService.IsHolidayAsync(request.ScheduleDatetime.Date))
        {
            return Result.Fail<string>(
                new Error("Appointment cannot be in holiday calendar")
                    .WithMetadata("Field",nameof(request.ScheduleDatetime)));
        }
        
        var agent = httpContextAccessor.HttpContext?.User;
        
        var appointment = new Appointment
        {
            CustomerName = request.CustomerName,
            AgentName = agent?.Identity?.Name ?? string.Empty,
            Location = request.Location,
            ScheduleDatetime = request.ScheduleDatetime.ToUniversalTime(),
            Token = Ulid.NewUlid().ToString()
        };
        
        dbContext.Appointments.Add(appointment);
        await dbContext.SaveChangesAsync();
        return Result.Ok(appointment.Token);
    }
}