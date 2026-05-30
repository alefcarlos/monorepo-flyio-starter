using Flyio.Demo.SharedKernel;
using Flyio.Demo.Todos.Contracts;
using Microsoft.Extensions.Logging;

namespace Flyio.Demo.Heart.Integrations;

internal class LogTodoIsDoneHandler : NotificationHandlerBase<TodoIsDoneIntegrationEvent>
{
    private readonly ILogger<LogTodoIsDoneHandler> _logger;

    public LogTodoIsDoneHandler(ILogger<LogTodoIsDoneHandler> logger)
    {
        _logger = logger;
    }

    protected override ValueTask HandleInternalAsync(TodoIsDoneIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling TodoIsDoneIntegrationEvent...");
        return default;
    }
}