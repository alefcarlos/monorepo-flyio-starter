using Flyio.Demo.ApiService.Endpoints.CreateTodo;
using Flyio.Demo.ApiService.Endpoints.GetTodo;

namespace Flyio.Demo.ApiService.Endpoints;

public static class Extensions
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("v1/todos")
            .WithTags("Todos")
            ;

        group.MapCreateTodo();
        group.MapGetTodo();

        return group;
    }
}