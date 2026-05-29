using Mediator;

namespace Flyio.Demo.Todos.Contracts;

public class TodoIsDoneIntegrationEvent : INotification
{
    public TodoIsDoneIntegrationEvent(TodoId id)
    {
        Id = id;
    }

    public TodoId Id { get; }
}