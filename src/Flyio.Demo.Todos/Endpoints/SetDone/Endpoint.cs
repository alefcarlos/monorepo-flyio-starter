using AlefCarlos.AspNetCoreDefaults.WebApi;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.UseCases.SetDone;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Flyio.Demo.Todos.Endpoints.SetDone;

public static class Extensions
{
    public static IEndpointRouteBuilder MapSetDone(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("{id:guid}:done", ExecuteAsync).RequireAuthorization();

        return endpoints;
    }

    private static async ValueTask<Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>> ExecuteAsync(IMediator mediator, Guid id)
    {
        var result = await mediator.Send(new SetTodoDoneCommand(new TodoId(id)));

        return result.ToNoContent();
    }
}