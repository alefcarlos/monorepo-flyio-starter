using Flyio.Demo.Todos.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Flyio.Demo.Todos;

public static class HostApplicationBuilderExtensions
{

    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddTodosModule()
        {
            builder.Services.AddDbContext<ITodosDbContext, TodosDbContext>((provider, opt) => opt.UseNpgsql(provider.GetRequiredService<IConfiguration>().GetConnectionString("Default")));
            builder.EnrichNpgsqlDbContext<TodosDbContext>();

            builder.Services.AddAuthorization(x => x.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("organization")
                .RequireAuthenticatedUser()
                .Build());

            //Exemplo de policies
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("todos_viewer", policy => policy.RequireRole("apiservice:viewer"))
                .AddPolicy("todos_writer", policy => policy.RequireRole("apiservice:writer"))
                ;

            return builder;
        }
    }
}