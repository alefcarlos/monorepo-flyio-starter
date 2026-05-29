using Mediator;

namespace Flyio.Demo.SharedKernel;

public abstract class DomainEventBase : INotification
{
  public DateTime DateOccurred { get; protected set; } = DateTime.Now;
}
