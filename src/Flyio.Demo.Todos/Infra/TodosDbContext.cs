using Flyio.Demo.SharedKernel.Entities;
using Flyio.Demo.SharedKernel.Infra.Interceptors;
using Flyio.Demo.Todos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Flyio.Demo.SharedKernel.Infra.EntityConfigurations;

namespace Flyio.Demo.Todos.Infra;

public class TodosDbContext : DbContext
{
    readonly UpdateAuditableEntitiesInterceptor _updateAuditableEntitiesInterceptor = new();

    public TodosDbContext(DbContextOptions<TodosDbContext> options) : base(options)
    {
    }

    public DbSet<AuditTrailEntity> AuditTrails { get; set; }

    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodosDbContext).Assembly);
    }
}