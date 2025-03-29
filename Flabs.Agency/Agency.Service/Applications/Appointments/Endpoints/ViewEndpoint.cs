using Agency.Service.Common;
using Agency.Service.Entities;
using Agency.Service.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Appointments;

public class ViewEndpoint(IHttpContextAccessor contextAccessor, ApplicationDbContext dbContext) 
    : EndpointWithoutRequest<IEnumerable<ViewAppointmentResponse>>
{
    public override void Configure()
    {
        Get("/appointments");
        Roles(RoleName.Agent);
        Summary(s =>
        {
            s.Summary = "View appointments assigned to the agent.";
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var agentName = contextAccessor?.HttpContext?.User?.Identity?.Name;
        var appointments = await dbContext.Appointments
            .Where(a => a.AgentName == agentName &&
                        a.ScheduleDatetime >= DateTimeOffset.UtcNow)
            .Select(p=> new Appointment()
            {
                CustomerName = p.CustomerName,
                Location = p.Location,
                ScheduleDatetime = p.ScheduleDatetime,
                Token = p.Token
            })
            .ToListAsync(cancellationToken: cancellationToken);
        var response = appointments
            .GroupBy(p => p.ScheduleDatetime.Date)
            .Select(p => new ViewAppointmentResponse
            {
                Date = p.Key,
                Appointments = p.Select(a => new ViewAppointment()
                {
                    CustomerName = a.CustomerName,
                    Location = a.Location,
                    ScheduleDatetime = a.ScheduleDatetime,
                    Token = a.Token
                }).ToList()
            });
        
        
        await SendOkAsync(response, cancellationToken);
    }
}