using AlefCarlos.AspNetCoreDefaults.WebApi;
using Flyio.Demo.ApiService.UseCases.Create;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Flyio.Demo.ApiService.Endpoints.CreateTodo;

public static class Extensions
{
    public static IEndpointRouteBuilder MapCreateTodo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("", CreateAsync);

        return endpoints;
    }

    private static async ValueTask<Results<Created, ValidationProblem, ProblemHttpResult>> CreateAsync(IMediator mediator, [FromBody] CreateTodoRequest request)
    {
        var result = await mediator.Send(new CreateTodoCommand(request.Name));

        return result.ToCreatedResult();
    }
}