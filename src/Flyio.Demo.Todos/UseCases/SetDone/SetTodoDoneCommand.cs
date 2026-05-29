using Ardalis.Result;
using Flyio.Demo.Todos.Contracts;
using Mediator;

namespace Flyio.Demo.Todos.UseCases.SetDone;

public record SetTodoDoneCommand(TodoId Id) : ICommand<Result>;
