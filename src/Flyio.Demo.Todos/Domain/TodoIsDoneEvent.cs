using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos.Contracts;

namespace Flyio.Demo.Todos.Domain;

public class TodoIsDoneEvent : DomainEventBase
{
    public TodoIsDoneEvent(TodoId id)
    {
        Id = id;
    }

    public TodoId Id { get; }
}