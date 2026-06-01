using Flyio.Demo.Todos.Endpoints.CreateTodo;
using Flyio.Demo.Todos.Endpoints.GetTodo;
using Flyio.Demo.Todos.Endpoints.SetDone;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing;

public static class Extensions
{
    public static IEndpointRouteBuilder MapTodosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("v1/todos")
            .WithTags("Todos")
            ;

        group.MapCreateTodo();
        group.MapGetTodo();
        group.MapSetDone();

        return group;
    }
}