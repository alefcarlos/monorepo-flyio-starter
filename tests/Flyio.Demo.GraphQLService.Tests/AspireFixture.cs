using System.Security.Claims;
using System.Text.Encodings.Web;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Flyio.Demo.GraphQLService.Tests.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flyio.Demo.GraphQLService.Tests;

public sealed class AspireFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private DistributedApplication _app = default!;

    public AspireWebApplicationFactory Factory { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Flyio_Demo_AppHost>();
        appHost.DisableContainerPersistence();

        _app = await appHost.BuildAsync(CancellationToken.None)
            .WaitAsync(DefaultTimeout, CancellationToken.None);

        await _app.StartAsync(CancellationToken.None)
            .WaitAsync(DefaultTimeout, CancellationToken.None);

        await _app.ResourceNotifications.WaitForResourceHealthyAsync("postgres")
            .WaitAsync(DefaultTimeout, CancellationToken.None);

        var connectionString = await _app.GetConnectionStringAsync("Default");

        Factory = new AspireWebApplicationFactory(connectionString!);
    }

    public async ValueTask DisposeAsync()
    {
        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}

[CollectionDefinition(nameof(AspireCollection))]
public class AspireCollection : ICollectionFixture<AspireFixture>;

internal static class DistributedApplicationTestingBuilderExtensions
{
    extension(IDistributedApplicationTestingBuilder builder)
    {
        internal void DisableContainerPersistence()
        {
            foreach (var resource in builder.Resources)
            {
                var lifetimeAnnotations = resource.Annotations.OfType<ContainerLifetimeAnnotation>().ToList();
                foreach (var item in lifetimeAnnotations)
                {
                    resource.Annotations.Remove(item);
                }

                var volumeAnnotations = resource.Annotations.OfType<ContainerMountAnnotation>()
                    .Where(x => x.Type == ContainerMountType.Volume)
                    .ToList();

                foreach (var item in volumeAnnotations)
                {
                    resource.Annotations.Remove(item);
                }

                if (resource is ContainerResource)
                {
                    builder.CreateResourceBuilder<ContainerResource>(resource.Name).WithLifetime(ContainerLifetime.Session);
                }
            }
        }
    }
}


public class AspireWebApplicationFactory : WebApplicationFactory<IGraphQLService>
{
    private readonly string _postgresConnectionString;
    public AspireWebApplicationFactory(string postgresConnectionString)
    {
        _postgresConnectionString = postgresConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", _postgresConnectionString);

        builder.ConfigureTestServices(services =>
        {
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
                new("organization", "acme"),
                new(ClaimTypes.Email, "test@acme.com")
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Bearer");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}