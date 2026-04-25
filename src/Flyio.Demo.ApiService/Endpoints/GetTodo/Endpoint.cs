using AlefCarlos.AspNetCoreDefaults.WebApi;
using Flyio.Demo.ApiService.Endpoints.Responses;
using Flyio.Demo.ApiService.UseCases;
using Flyio.Demo.ApiService.UseCases.Get;
using Flyio.Demo.ApiService.UseCases.GetAll;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Flyio.Demo.ApiService.Endpoints.GetTodo;

public static class Extensions
{
    public static IEndpointRouteBuilder MapGetTodo(this IEndpointRouteBuilder endpoints)
    {
        // endpoints.MapGet("{id:guid}", GetByIdAsync);
        endpoints.MapGet("", GetAllAsync);

        return endpoints;
    }

    private static async ValueTask<Ok<IEnumerable<TodoResponse>>> GetAllAsync(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllTodosQuery());

        return result.ToOkOnlyResult((list) => list.Select(TodoResponse.FromEntity));
    }

    private static async ValueTask<Results<Ok<TodoResponse>, NotFound, ProblemHttpResult>> GetByIdAsync(IMediator mediator, Guid id)
    {
        var result = await mediator.Send(new GetTodoQuery(new(id)));

        return result.ToGetByIdResult(TodoResponse.FromEntity);
    }
}