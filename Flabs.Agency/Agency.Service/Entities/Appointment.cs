using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Agency.Service.Entities;

public class Appointment : IAuditColumn
{
    public Guid Id { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset ScheduleDatetime { get; set; }
    public string Token { get; set; } =  string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? ModifiedTime { get; set; }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property<string>(e => e.CustomerName)
            .HasMaxLength(100);
        builder.Property<string>(e => e.AgentName)
            .HasMaxLength(100);
        builder.Property<string>(e => e.Location)
            .HasMaxLength(500);
        builder.Property<string>(e => e.Token)
            .HasMaxLength(26);
        builder.Property(e=> e.ModifiedTime)
            .IsConcurrencyToken();
    }
}