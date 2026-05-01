using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Flyio.Demo.Web.SessionManagement;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        if (identity.HasClaim(c => c.Type == ClaimTypes.Role))
            return Task.FromResult(principal);

        var resourceAccessClaim = principal.FindFirst("resource_access")?.Value;

        if (string.IsNullOrWhiteSpace(resourceAccessClaim))
            return Task.FromResult(principal);


        using var doc = JsonDocument.Parse(resourceAccessClaim);

        foreach (var client in doc.RootElement.EnumerateObject())
        {
            var clientId = client.Name;

            if (client.Value.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var roleValue = role.GetString();

                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        var formatted = $"{clientId}:{roleValue}";

                        identity.AddClaim(new Claim(ClaimTypes.Role, formatted));
                    }
                }
            }
        }

        return Task.FromResult(principal);
    }
}