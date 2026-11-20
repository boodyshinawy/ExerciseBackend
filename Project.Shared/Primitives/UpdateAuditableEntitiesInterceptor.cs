using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace Project.Shared;

public class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateAuditableEntitiesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;
        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<IAuditableEntity>> entries = context.ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        string? userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        foreach (EntityEntry<IAuditableEntity> entry in entries)
        {
            entry.Entity.OldState = entry.Entity.NewState;
            entry.Entity.NewState = entry.State switch
            {
                EntityState.Added => StateType.Created,
                EntityState.Modified => StateType.Updated,
                EntityState.Deleted => StateType.Deleted,
                _ => StateType.Restored,
            };

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.CreatedBy = userName;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedDate = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = userName;
            }

            if (entry.Entity.IsDeleted || entry.State == EntityState.Deleted)
            {
                entry.Entity.NewState = StateType.Deleted;
                entry.Entity.LastModifiedBy = userName;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
