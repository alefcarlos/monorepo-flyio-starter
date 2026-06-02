using Microsoft.AspNetCore.Authorization;

namespace Flyio.Demo.Module.SharedKernel.Endpoints;

public sealed record TenantRequirement : IAuthorizationRequirement
{
    public override string ToString()
    {
        return $"{nameof(TenantRequirement)}: Requires 'organization' claim.";

    }
}
