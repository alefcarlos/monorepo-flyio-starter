using Mediator;

namespace Flyio.Demo.SharedKernel;

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class DomainEventBase : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.Now;
}

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEventsAsync(IEnumerable<IHaveDomainEvents> entitiesWithEvents);
}

public abstract record IntegrationEventBase : INotification
{
  public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
}