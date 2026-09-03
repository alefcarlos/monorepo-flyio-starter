using Flyio.Demo.Module.SharedKernel.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Flyio.Demo.Todos.Infra;

public class MigrationDbTask : IHostedService
{
    private readonly IServiceProvider _sp;

    public MigrationDbTask(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _sp.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<TodosDbContext>>();

        using var context = new TodosDbContext(options, dispatcher: null, tenantGetter: new TenantStub());
        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

file class TenantStub : ITenantGetter
{
    public string GetCurrentTenant() => "stub";
}