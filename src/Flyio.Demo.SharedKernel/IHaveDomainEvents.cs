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

public class DomainEventDispatcher : IDomainEventDispatcher
{
  private readonly IMediator _mediator;

  public DomainEventDispatcher(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task DispatchAndClearEventsAsync(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
  {
    foreach (var entity in entitiesWithEvents)
    {
      var events = entity.DomainEvents.ToArray();
      entity.ClearDomainEvents();
      
      foreach (var domainEvent in events)
      {
        await _mediator.Publish(domainEvent).ConfigureAwait(false);
      }
    }
  }
}