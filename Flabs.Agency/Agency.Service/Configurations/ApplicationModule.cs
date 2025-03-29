using Agency.Service.Appointments;
using Agency.Service.Authentication;
using Agency.Service.Holidays;

namespace Agency.Service.Configurations;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IHolidayService, HolidayService>();
        return services;
    }
}