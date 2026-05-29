using AlefCarlos.AspNetCoreDefaults.WebApi;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Endpoints.CreateTodo;
using Flyio.Demo.Todos.Endpoints.GetTodo;
using Flyio.Demo.Todos.UseCases.SetDone;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

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

        group.MapPost("{id:guid}:done", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new SetTodoDoneCommand(new TodoId(id)));

            return result.ToOkOrNotFoundResult("SetDone");
        });

        return group;
    }
}

public static class ResultExtensions
{
    public static Results<Ok, NotFound, ProblemHttpResult> ToOkOrNotFoundResult(this Result result, string operationName)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                return TypedResults.Ok();
            case ResultStatus.NotFound:
                return TypedResults.NotFound();
            default:
                {
                    string title = operationName + " failed";
                    return TypedResults.Problem(string.Join("; ", result.Errors), null, 400, title);
                }
        }
    }

    public static Results<Ok<TResponse>, NotFound, ProblemHttpResult> ToOkOrNotFoundResult<TValue, TResponse>(Result<TValue> result, Func<TValue, TResponse> mapResponse, string operationName)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                return TypedResults.Ok(mapResponse(result.Value));
            case ResultStatus.NotFound:
                return TypedResults.NotFound();
            default:
                {
                    string title = operationName + " failed";
                    return TypedResults.Problem(string.Join("; ", result.Errors), null, 400, title);
                }
        }
    }

}