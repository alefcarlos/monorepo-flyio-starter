using Flyio.Demo.GraphQLService.Tests.Client;
using Flyio.Demo.Todos.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Flyio.Demo.GraphQLService.Tests;

public class UnauthorizedWebApplicationFactory : WebApplicationFactory<IGraphQLService>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<SkipMigrations>();
            // services.AddSingleton<IStockService, StockServiceFake>();
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
}