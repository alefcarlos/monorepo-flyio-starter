using System.Security.Claims;
using System.Text.Encodings.Web;
using Flyio.Demo.GraphQLService.Tests.Client;
using Flyio.Demo.Todos.Infra;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flyio.Demo.GraphQLService.Tests;

file class Stub : ISkipMigrations;

public class AuthorizedWebApplicationFactory : WebApplicationFactory<IGraphQLService>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ISkipMigrations, Stub>();
            services.Configure<AuthenticationOptions>(x => x.SchemeMap["Bearer"].HandlerType = typeof(ViewerAuthenticationHandler));
        });
    }

    public IDemoGraphQLClient CreateGraphqQLClient()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddDemoGraphQLClient()
            .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = Server.BaseAddress;
                },
                c =>
                {
                    c.ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler());
                });

        return serviceCollection
            .BuildServiceProvider()
            .GetRequiredService<IDemoGraphQLClient>();
    }

    private class ViewerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ViewerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, "apiservice:viewer"),
                new("organization", "acme")
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Bearer");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}