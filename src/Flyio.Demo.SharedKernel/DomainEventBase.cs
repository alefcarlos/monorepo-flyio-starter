using Mediator;

namespace Flyio.Demo.SharedKernel;

public abstract record DomainEventBase : INotification
{
  public DateTime DateOccurred { get; } = DateTime.Now;
}
