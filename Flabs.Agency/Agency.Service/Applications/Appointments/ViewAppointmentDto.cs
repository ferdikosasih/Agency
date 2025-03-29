namespace Agency.Service.Appointments;
public record ViewAppointmentResponse
{
    public DateTimeOffset Date { get; set; }
    public List<ViewAppointment> Appointments { get; set; } = new();
}

public record ViewAppointment
{
    public string CustomerName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset ScheduleDatetime { get; set; }
    public string Token { get; set; } = string.Empty;
}