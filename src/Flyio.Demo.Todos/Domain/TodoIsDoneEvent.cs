using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos.Contracts;

namespace Flyio.Demo.Todos.Domain;

public record TodoIsDoneEvent(TodoId Id) : DomainEventBase;