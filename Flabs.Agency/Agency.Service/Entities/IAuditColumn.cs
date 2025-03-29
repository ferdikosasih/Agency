namespace Agency.Service.Entities;

public interface IAuditColumn
{
    DateTimeOffset CreatedTime { get; set; }
    DateTimeOffset? ModifiedTime { get; set; }
}