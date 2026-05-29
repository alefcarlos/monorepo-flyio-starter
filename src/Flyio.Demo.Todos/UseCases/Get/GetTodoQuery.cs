using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.Get;

public record GetTodoQuery(TodoId Id) : IQuery<Result<TodoEntity>>;
