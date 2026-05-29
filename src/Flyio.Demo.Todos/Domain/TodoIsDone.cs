using Flyio.Demo.Todos.Contracts;
using Mediator;

namespace Flyio.Demo.Todos.Domain;

public record TodoIsDone(TodoId Id, DateTimeOffset Date) : INotification;