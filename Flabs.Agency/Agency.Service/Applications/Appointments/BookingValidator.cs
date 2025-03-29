using Agency.Service.Infrastructure;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Appointments;

public class BookingValidator : Validator<AppointmentRequest>
{
    private const int MaxAppointmentPerDay = 3;
    public BookingValidator()
    {
        RuleFor(x=> x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(100)
            .WithMessage("Customer name must not exceed 100 characters.");
        
        RuleFor(x=> x.Location)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(500)
            .WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.ScheduleDatetime)
            .NotEmpty()
            .WithMessage("Schedule datetime is required.")
            .Must(BeInTheFuture)
            .WithMessage("Appointment must be at least 15 minutes in the future.");
    }

    private bool BeInTheFuture(DateTimeOffset scheduleDateTime)
    {
        return scheduleDateTime.UtcDateTime > DateTimeOffset.UtcNow.AddMinutes(15);
    }
}