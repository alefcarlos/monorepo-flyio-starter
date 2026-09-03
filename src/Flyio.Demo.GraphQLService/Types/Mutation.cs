using Ardalis.Result;
using Flyio.Demo.GraphQLService.Types.Errors;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Endpoints.Responses;
using Flyio.Demo.Todos.UseCases.Create;
using Flyio.Demo.Todos.UseCases.SetDone;
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
        return (await mediator.Send(new CreateTodoCommand(name), ct))
            .EnsureSuccess()
            .Map(TodoResponse.FromEntity);
    }

    [Authorize]
    [Error(typeof(GraphQLConflictException))]
    [Error(typeof(GraphQLNotFoundException))]
    [Error(typeof(GraphQLResultErrorException))]
    public static async Task<TodoResponse> SetTodoDoneAsync(IMediator mediator, Guid id, CancellationToken ct)
    {
        return (await mediator.Send(new SetTodoDoneCommand(new TodoId(id)), ct))
            .EnsureSuccess()
            .Map(TodoResponse.FromEntity);
    }
}