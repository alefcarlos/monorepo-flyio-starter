using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.Get;

public record GetTodoQuery(TodoId Id) : IQuery<Result<TodoEntity>>;
