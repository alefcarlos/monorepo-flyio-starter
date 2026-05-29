using Ardalis.Result;
using Flyio.Demo.Todos.Domain;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.GetAll;

public record GetAllTodosQuery() : IQuery<Result<List<TodoEntity>>>;
