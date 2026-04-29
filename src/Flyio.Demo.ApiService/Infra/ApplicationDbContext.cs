using Flyio.Demo.ApiService.Entities;
using Flyio.Demo.ApiService.Infra.Interceptors;
using Flyio.Demo.ApiService.UseCases;
using Microsoft.EntityFrameworkCore;

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

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}