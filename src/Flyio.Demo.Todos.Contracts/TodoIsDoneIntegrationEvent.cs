using Flyio.Demo.SharedKernel;

namespace Flyio.Demo.Todos.Contracts;

public record TodoIsDoneIntegrationEvent(TodoId Id) : IntegrationEventBase;