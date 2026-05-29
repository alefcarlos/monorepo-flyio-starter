using AlefCarlos.AspNetCoreDefaults.WebApi;
using Flyio.Demo.Todos.UseCases.Create;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Flyio.Demo.Todos.Endpoints.CreateTodo;

public static class Extensions
{
    public static IEndpointRouteBuilder MapCreateTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("", CreateAsync).RequireAuthorization("todos_writer");

        return endpoints;
    }

    private static async ValueTask<Results<Created<Guid>, ValidationProblem, ProblemHttpResult>> CreateAsync(IMediator mediator, [FromBody] CreateTodoRequest request)
    {
        var result = await mediator.Send(new CreateTodoCommand(request.Name));

        return result.ToCreatedResult((value) => $"/v1/todos/{value.Value}", (id) => id.Value);
    }
}