namespace Flyio.Demo.SharedKernel;

public interface IHaveDomainEvents
{
  IEnumerable<DomainEventBase> DomainEvents { get; }
  void ClearDomainEvents();
}