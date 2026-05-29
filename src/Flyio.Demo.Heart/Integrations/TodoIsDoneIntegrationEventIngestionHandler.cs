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

    // public async ValueTask Handle(OrderCreatedIntegrationEvent notification, CancellationToken ct)
    // {
    //     _logger.LogInformation("Handling order created event to populate reporting database...");

    //     var orderItems = notification.OrderDetails.OrderItems;
    //     int year = notification.OrderDetails.DateCreated.Year;
    //     int month = notification.OrderDetails.DateCreated.Month;

    //     foreach (var item in orderItems)
    //     {
    //         // look up book details to get author and title
    //         // TODO: Implement Materialized View or other cache
    //         var bookDetailsQuery = new BookDetailsQuery(item.BookId);
    //         var result = await _mediator.Send(bookDetailsQuery);

    //         if (!result.IsSuccess)
    //         {
    //             _logger.LogWarning("Issue loading book details for {id}", item.BookId);
    //             continue;
    //         }

    //         string author = result.Value.Author;
    //         string title = result.Value.Title;

    //         var sale = new BookSale
    //         {
    //             Author = author,
    //             BookId = item.BookId,
    //             Month = month,
    //             Title = title,
    //             Year = year,
    //             TotalSales = item.Quantity * item.UnitPrice,
    //             UnitsSold = item.Quantity
    //         };

    //         await _orderIngestionService.AddOrUpdateMonthlyBookSalesAsync(sale);
    //     }
    // }
}