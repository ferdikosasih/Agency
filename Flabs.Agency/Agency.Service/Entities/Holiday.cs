using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agency.Service.Entities;

public class Holiday
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    [JsonPropertyName("public")]
    public bool Public { get; set; }
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;
}

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Date)
            .HasColumnType("DATE");
        builder.Property<string>(e => e.Name)
            .HasMaxLength(100);
        builder.Property<string>(e => e.Country)
            .HasMaxLength(100);
        builder.Property<string>(e => e.Uuid)
            .HasMaxLength(36);
    }
}