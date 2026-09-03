using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.SetDone;

public record SetTodoDoneCommand(TodoId Id) : ICommand<Result<TodoEntity>>;
