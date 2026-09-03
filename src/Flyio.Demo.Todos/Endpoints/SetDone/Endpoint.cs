using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.UseCases.SetDone;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Flyio.Demo.Todos.Endpoints.SetDone;

public static class Endpoints
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

public static class ResultExtesions
{
    public static Results<
     NoContent,
     NotFound,
     ValidationProblem,
     ProblemHttpResult>
     ToNoContent<TValue>(this Result<TValue> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok =>
                TypedResults.NoContent(),

            ResultStatus.NotFound =>
                TypedResults.NotFound(),

            ResultStatus.Invalid =>
                CreateValidationProblem(result),

            _ =>
                CreateProblem(result, "Operation failed")
        };
    }

    private static ValidationProblem CreateValidationProblem(Ardalis.Result.IResult result)
    {
        return TypedResults.ValidationProblem(
            result.ValidationErrors
                .GroupBy(x => x.Identifier ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()));
    }

    private static ProblemHttpResult CreateProblem(
        Ardalis.Result.IResult result,
        string title)
    {
        return TypedResults.Problem(
            title: title,
            detail: string.Join("; ", result.Errors),
            statusCode: StatusCodes.Status400BadRequest);
    }
}