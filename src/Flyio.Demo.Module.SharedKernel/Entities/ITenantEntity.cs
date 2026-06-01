//https://github.com/PlatformPlatform/platformplatform/tree/main/shared-kernel
namespace Flyio.Demo.Module.SharedKernel.Entities;

public interface ITenantEntity
{
    string TenantId { get; }

    void SetTenantId(string tenant);
}