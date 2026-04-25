using Flyio.Demo.ApiService.Entities;
using Flyio.Demo.ApiService.UseCases;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.ApiService.Infra;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}