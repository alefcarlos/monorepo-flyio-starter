using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Flyio.Demo.Todos.Domain;
using Flyio.Demo.SharedKernel;
using Flyio.Demo.Module.SharedKernel.Infra.Interceptors;
using Flyio.Demo.Module.SharedKernel.Entities;
using Flyio.Demo.Module.SharedKernel.Infra.EntityConfigurations;
using Flyio.Demo.Module.SharedKernel.Infra;

namespace Flyio.Demo.Todos.Infra;

public interface ITodosDbContext
{
    DbSet<TodoEntity> Todos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IQueryable<T> SqlQuery<T>(string sql) where T : notnull;
}

internal class TodosDbContext : DbContext, ITodosDbContext
{
    private readonly string _tenantId;
    private readonly IDomainEventDispatcher? _dispatcher;
    readonly UpdateAuditableEntitiesInterceptor _updateAuditableEntitiesInterceptor = new();
    readonly SetTenantIdInterceptor _setTenantIdInterceptor = new();

    public TodosDbContext(DbContextOptions<TodosDbContext> options,
        IDomainEventDispatcher? dispatcher,
        ITenantGetter tenantGetter) : base(options)
    {
        _dispatcher = dispatcher;
        _tenantId = tenantGetter.GetCurrentTenant();
    }

    public DbSet<AuditTrailEntity> AuditTrails { get; set; }

    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_setTenantIdInterceptor);
        optionsBuilder.AddInterceptors(_updateAuditableEntitiesInterceptor);
        optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug),
                                                                           (RelationalEventId.CommandExecuted, LogLevel.Debug),
                                                                           (CoreEventId.ContextInitialized, LogLevel.Debug)
                                                                           ));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();

        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();

        configurationBuilder
            .Properties<DateTimeOffset?>()
            .HaveConversion<NullableDateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Todos");

        modelBuilder.Entity<TodoEntity>().HasQueryFilter(b => b.TenantId == _tenantId);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodosDbContext).Assembly);
    }

    /// <summary>
    /// This is needed for domain events to work
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null) return result;

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<IHaveDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
        .ToArray();

        await _dispatcher.DispatchAndClearEventsAsync(entitiesWithEvents);

        return result;
    }

    public IQueryable<T> SqlQuery<T>(string sql) where T : notnull
    {
        return Database.SqlQueryRaw<T>(sql);
    }
}