using Duende.AccessTokenManagement.OpenIdConnect;
using Flyio.Demo.Web.SessionManagement;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.Extensions.DependencyInjection;

public static class UserSessionManagementExtensions
{
    public static IServiceCollection AddUserSessionManagement(this IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;

                options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

                options.Scope.Add("offline_access");
                options.SaveTokens = true;

                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "given_name";

                options.EventsType = typeof(OidcEvents);
            })
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "/access-denied";

                options.EventsType = typeof(CookieEvents);
            })
            ;

        services.AddTransient<CookieEvents>();
        services.AddTransient<OidcEvents>();

        services.AddOpenIdConnectAccessTokenManagement()
            .AddBlazorServerAccessTokenManagement<ServerSideTokenStore>();

        services.AddAuthorization();

        return services;
    }
}