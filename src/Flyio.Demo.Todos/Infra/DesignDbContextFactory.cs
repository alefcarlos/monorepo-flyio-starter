using Flyio.Demo.Module.SharedKernel.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flyio.Demo.Todos.Infra;

/// <summary>
/// This class is just for using with EFTools
/// </summary>
internal class DesignDbContextFactory : IDesignTimeDbContextFactory<TodosDbContext>
{
    public TodosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodosDbContext>();
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=br01;User ID=sa;Password=1q2w3e4r@#$;Trusted_Connection=False; TrustServerCertificate=True;");

        return new TodosDbContext(optionsBuilder.Options, null, TenatnGetterStub.Value);
    }
}

file sealed class TenatnGetterStub : ITenantGetter
{
    public static readonly TenatnGetterStub Value = new();
    public string GetCurrentTenant()
    {
        return "tenant";
    }
}