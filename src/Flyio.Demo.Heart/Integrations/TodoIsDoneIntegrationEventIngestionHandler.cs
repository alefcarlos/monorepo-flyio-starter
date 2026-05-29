using Flyio.Demo.Todos.Contracts;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Flyio.Demo.Heart.Integrations;

public class TodoIsDoneIntegrationEventIngestionHandler : INotificationHandler<TodoIsDoneIntegrationEvent>
{
    private readonly ILogger<TodoIsDoneIntegrationEventIngestionHandler> _logger;

    public TodoIsDoneIntegrationEventIngestionHandler(ILogger<TodoIsDoneIntegrationEventIngestionHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(TodoIsDoneIntegrationEvent notification, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Handling TodoIsDoneIntegrationEvent...");

        return ValueTask.CompletedTask;
    }
}