using Agency.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Agency.Service.Infrastructure;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData
        , InterceptionResult<int> result
        , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        if (eventData.Context is not null)
        {
            UpdateAuditablesEntities(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        if (eventData.Context is not null)
        {
            UpdateAuditablesEntities(eventData.Context);
        }
        return base.SavedChanges(eventData, result);
    }

    private void UpdateAuditablesEntities(DbContext context)
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        var entities = context.ChangeTracker.Entries<IAuditColumn>().ToList();
        foreach (EntityEntry<IAuditColumn> entry in entities)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditColumn.CreatedTime)).CurrentValue = utcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditColumn.ModifiedTime)).CurrentValue = utcNow;
            }
        }
    }
}