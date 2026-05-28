using Flyio.Demo.ApiService.Entities;
using Flyio.Demo.ApiService.Infra.EntityConfigurations;
using Flyio.Demo.ApiService.Infra.Interceptors;
using Flyio.Demo.ApiService.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Flyio.Demo.ApiService.Infra;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    readonly UpdateAuditableEntitiesInterceptor _updateAuditableEntitiesInterceptor = new();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}