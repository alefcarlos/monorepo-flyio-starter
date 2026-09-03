using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Create;

public record CreateTodoCommand(string Name) : ICommand<Result<TodoEntity>>;
