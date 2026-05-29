using Flyio.Demo.Todos.Contracts;
using Flyio.Demo.Todos.Domain;
using Mediator;

namespace Flyio.Demo.Todos.Integrations;

public class PublishTodoIsDoneIntegrationEventEventHandler : INotificationHandler<TodoIsDoneEvent>
{
    private readonly IMediator _mediator;

    public PublishTodoIsDoneIntegrationEventEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async ValueTask Handle(TodoIsDoneEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new TodoIsDoneIntegrationEvent(notification.Id);

        await _mediator.Publish(integrationEvent);
    }
}