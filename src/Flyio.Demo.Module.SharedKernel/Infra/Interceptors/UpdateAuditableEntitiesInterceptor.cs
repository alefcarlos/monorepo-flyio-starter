using System.Security.Claims;
using System.Text.Json;
using Flyio.Demo.Module.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flyio.Demo.Module.SharedKernel.Infra.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateEntities(DbContextEventData eventData)
    {
        var dbContext = eventData.Context ?? throw new NullReferenceException();

        var audibleEntities = dbContext.ChangeTracker.Entries<IAuditableEntity>().ToList();

        var user = dbContext.GetService<IHttpContextAccessor>().HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;

        var entries = dbContext.Set<AuditTrailEntity>();

        foreach (var entityEntry in audibleEntities)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.UpdateCreatedBy(user);
                    break;
                case EntityState.Modified:
                    entityEntry.Entity.UpdateModified(user, DateTime.UtcNow);
                    break;
            }

            var audit = new Audit(entityEntry, user);
            entries.Add(audit.ToEntity());
        }
    }
}

file record Audit(EntityEntry<IAuditableEntity> Entry, string UserId);

file static class AuditExtensions
{
    public static AuditTrailEntity ToEntity(this Audit audit)
    {
        var keyValues = new Dictionary<string, object>();
        var oldValues = new Dictionary<string, object>();
        var newValues = new Dictionary<string, object>();
        var changedColumns = new HashSet<string>();

        var type = audit.Entry.State switch
        {
            EntityState.Added => ETrailType.Create,
            EntityState.Modified => ETrailType.Edit,
            _ => ETrailType.Undefined
        };

        // Skip temp fields (that will be assigned automatically by ef core engine, for example: when inserting an entity
        foreach (var property in audit.Entry.Properties.Where(x => !x.IsTemporary))
        {
            var propertyName = property.Metadata.Name;

            if (property.Metadata.IsPrimaryKey())
            {
                keyValues[propertyName] = property.CurrentValue!;
                continue;
            }

            switch (audit.Entry.State)
            {
                case EntityState.Added:
                    newValues[propertyName] = GetCurrentValue(property);
                    break;

                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        changedColumns.Add(propertyName);
                        oldValues[propertyName] = GetOriginalValue(property);
                        newValues[propertyName] = GetCurrentValue(property);
                    }
                    break;
            }

        }

        return new AuditTrailEntity
        {
            TrailType = type,
            EntityName = audit.Entry.Entity.GetType().Name,
            UserIdentifier = audit.UserId,
            AffectedColumns = JsonSerializer.Serialize(changedColumns),
            NewValues = JsonSerializer.Serialize(newValues),
            OldValues = JsonSerializer.Serialize(oldValues),
            PrimaryKey = JsonSerializer.Serialize(keyValues),
        };
    }

    private static object GetCurrentValue(PropertyEntry property)
    {
        return property.CurrentValue!;
    }

    private static object GetOriginalValue(PropertyEntry property)
    {
        return property.OriginalValue!;
    }
}