using Flyio.Demo.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.ApiService.UseCases;

public interface IApplicationDbContext
{
    DbSet<TodoEntity> Todos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}