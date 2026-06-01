using Flyio.Demo.Module.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flyio.Demo.Module.SharedKernel.Infra.Interceptors;

public sealed class SetTenantIdInterceptor : SaveChangesInterceptor
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

        var tenantGetter = dbContext.GetService<ITenantGetter>();
        var tenant = tenantGetter.GetCurrentTenant();
        var entries = dbContext.ChangeTracker.Entries<ITenantEntity>()
                                                        .Where(e => e is { State: EntityState.Added });

        foreach (var entry in entries)
        {
            if (entry.Entity.TenantId is not null) continue;

            entry.Entity.SetTenantId(tenant);
        }
    }
}