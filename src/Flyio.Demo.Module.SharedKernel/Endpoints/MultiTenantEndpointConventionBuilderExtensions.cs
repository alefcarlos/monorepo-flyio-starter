using Flyio.Demo.Module.SharedKernel.Endpoints;
using Scalar.AspNetCore;

namespace Microsoft.AspNetCore.Builder;
public static class MultiTenantEndpointConventionBuilderExtensions
{
    public static TBuilder RequireTenant<TBuilder>(this TBuilder builder, params string[] tenants) where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        
        builder.WithBadge("MultiTenant", BadgePosition.Before);

        builder.RequireAuthorization()
            .Add((endpointBuilder) =>
            {
                endpointBuilder.Metadata.Add(new EnableTenant(tenants));
            });

        return builder;
    }
}
