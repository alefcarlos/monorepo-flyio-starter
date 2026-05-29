using Ardalis.Result;
using Flyio.Demo.Todos.Entities;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Get;

public record GetTodoQuery(TodoId Id) : IQuery<Result<TodoEntity>>;
