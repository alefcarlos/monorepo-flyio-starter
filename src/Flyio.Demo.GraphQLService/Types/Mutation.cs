using Ardalis.Result;
using Flyio.Demo.GraphQLService.Types.Errors;
using Flyio.Demo.Todos.Endpoints.Responses;
using Flyio.Demo.Todos.UseCases.Create;
using HotChocolate.Authorization;
using Mediator;

namespace Flyio.Demo.GraphQLService.Types;

[MutationType]
public static partial class Mutation
{
    [Authorize]
    [Error(typeof(GraphQLValidationException))]
    [Error(typeof(GraphQLConflictException))]
    [Error(typeof(GraphQLResultErrorException))]
    public static async Task<TodoResponse> AddTodoAsync(IMediator mediator, string name, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateTodoCommand(name), ct);

        return result.EnsureSuccess().Map((data) => TodoResponse.FromEntity(result.Value));
    }
}