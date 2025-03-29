namespace Agency.Service.Appointments;

/// <summary>
/// Create appointment request
/// </summary>
public record AppointmentRequest
{
    /// <summary>
    /// Customer name for appointment request
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    /// <summary>
    /// Address for appointment
    /// </summary>
    public string Location { get; set; } = string.Empty;
    /// <summary>
    /// Date and time for appointment
    /// </summary>
    public DateTimeOffset ScheduleDatetime { get; set; }
}
public record AppointmentResponse
{
    /// <summary>
    /// Token issued when appointment successfully created
    /// </summary>
    public string Token { get; set; } = string.Empty;
}