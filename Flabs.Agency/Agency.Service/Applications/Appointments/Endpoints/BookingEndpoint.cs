using Agency.Service.Common;
using FastEndpoints;
using FluentValidation.Results;

namespace Agency.Service.Appointments;

public class BookingEndpoint(
    IBookingService bookingService) 
    : Endpoint<AppointmentRequest, AppointmentResponse>
{
    public override void Configure()
    {
        Post("/appointments");
        Description(builder =>
        {
            builder
                .Produces<AppointmentResponse>(200)
                .Produces(400)
                .Produces(401)
                .Produces(403)
                .Produces(500);
        });
        Summary(s =>
        {
            s.Summary = "Book Appointment";
            s.ExampleRequest = new AppointmentRequest()
            {
                CustomerName = "John Doe",
                Location = "123 Main St",
                ScheduleDatetime = DateTimeOffset.Now.ToLocalTime()
            };
            s.ResponseExamples[200] = new AppointmentResponse()
            {
                Token = "01JQDRV7C7RE9QMCKS5A672YPW"
            };

        });
        Roles(RoleName.Agent);
    }

    public override async Task HandleAsync(
        AppointmentRequest request
        , CancellationToken cancellationToken)
    {
        var result = await bookingService.MakeAppointmentAsync(request);
        if (result.IsSuccess)
        {
            await SendOkAsync(new()
            {
                Token = result.Value
            }, cancellationToken);
        }
        else
        {
            var failures = result.Errors
                .Select(error => new ValidationFailure(
                    error.Metadata.TryGetValue("Field", out var field) ? field?.ToString() ?? "general" : "general",
                    error.Message))
                .ToList();
            failures.ForEach(AddError);

            await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
        }
    }
}