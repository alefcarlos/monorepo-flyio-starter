namespace Flyio.Demo.SharedKernel;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEventsAsync(IEnumerable<IHaveDomainEvents> entitiesWithEvents);
}
