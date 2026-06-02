using System.Security.Claims;
using Flyio.Demo.Module.SharedKernel.Endpoints;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authorization;

public sealed class TenantRequirementHandler
    : AuthorizationHandler<TenantRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();

            var tenantMetadata = endpoint?.Metadata.GetMetadata<EnableTenant>();

            if (tenantMetadata is null)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var tenantClaim = context.User.FindFirstValue("organization");

            if (tenantClaim is null)
            {
                return Task.CompletedTask;
            }

            if (tenantMetadata.AllowedTenants.Length == 0)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (tenantMetadata.AllowedTenants.Contains(tenantClaim, StringComparer.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder TenantWhenRequired(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new TenantRequirement());
    }
}