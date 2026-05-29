using Mediator;

namespace Flyio.Demo.SharedKernel;

public abstract record IntegrationEventBase : INotification
{
  public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
}
