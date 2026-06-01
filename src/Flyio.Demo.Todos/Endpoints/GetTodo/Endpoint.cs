using AlefCarlos.AspNetCoreDefaults.WebApi;
using Flyio.Demo.Todos.Endpoints.Responses;
using Flyio.Demo.Todos.UseCases.Get;
using Flyio.Demo.Todos.UseCases.GetAll;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Flyio.Demo.Todos.Endpoints.GetTodo;

public static class Extensions
{
    public static IEndpointRouteBuilder MapGetTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("{id:guid}", GetByIdAsync).RequireAuthorization();
        endpoints.MapGet("", GetAllAsync).RequireAuthorization();

        return endpoints;
    }

    private static async ValueTask<Results<Ok<IEnumerable<TodoResponse>>, ValidationProblem, ProblemHttpResult>> GetAllAsync(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllTodosQuery());

        return result.ToOk((list) => list.Select(TodoResponse.FromEntity));
    }

    private static async ValueTask<Results<Ok<TodoResponse>, ValidationProblem, ProblemHttpResult>> GetByIdAsync(IMediator mediator, Guid id)
    {
        var result = await mediator.Send(new GetTodoQuery(new(id)));

        return result.ToOk(TodoResponse.FromEntity);
    }
}