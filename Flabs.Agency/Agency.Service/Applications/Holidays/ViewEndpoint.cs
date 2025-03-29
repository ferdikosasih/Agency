using Agency.Service.Common;
using Agency.Service.Entities;
using Agency.Service.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Holidays;

public class ViewEndpoint(ApplicationDbContext dbContext) 
    : EndpointWithoutRequest<HolidayResponse?>
{
    public override void Configure()
    {
        Get("/holidays");
        Roles(RoleName.Agent);
        Summary(s =>
        {
            s.Summary = "View holidays calendar.";
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var holidays = await dbContext.Holidays
            .Where(p => p.Date.Year == DateTime.Now.Year)
            .Select(p => new Holiday()
            {
                Date = p.Date,
                Name = p.Name,
                Public = p.Public,
                Country = p.Country,
                Uuid = p.Uuid
            })
            .ToListAsync(cancellationToken);
        var response = new HolidayResponse
        {
            Holidays = holidays
        };
        await SendOkAsync(response, cancellationToken);
    }
}