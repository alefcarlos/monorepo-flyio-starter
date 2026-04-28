using Ardalis.Result;
using Flyio.Demo.ApiService.Entities;
using Mediator;

namespace Flyio.Demo.ApiService.UseCases.Create;

public record CreateTodoCommand(string Name) : ICommand<Result<TodoId>>;
