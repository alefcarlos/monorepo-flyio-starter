using Ardalis.Result;
using Flyio.Demo.Todos.Entities;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Create;

public record CreateTodoCommand(string Name) : ICommand<Result<TodoId>>;
