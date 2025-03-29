using Agency.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Holidays;

public interface IHolidayService
{
    Task<bool> IsHolidayAsync(DateTime date);
}

public class HolidayService(ApplicationDbContext dbContext) : IHolidayService
{
    public async Task<bool> IsHolidayAsync(DateTime date)
    {
        return await dbContext.Holidays
            .AnyAsync(p => p.Date == DateOnly.FromDateTime(date));
    }
}