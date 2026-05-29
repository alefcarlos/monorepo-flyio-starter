using Mediator;

namespace Flyio.Demo.Todos.Contracts;

public record TodoIsDoneIntegrationEvent(TodoId Id) : INotification;