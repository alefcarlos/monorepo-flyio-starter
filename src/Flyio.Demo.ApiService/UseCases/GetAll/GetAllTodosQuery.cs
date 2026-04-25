using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.GetAll;

public record GetAllTodosQuery() : IQuery<Result<List<TodoEntity>>>;
