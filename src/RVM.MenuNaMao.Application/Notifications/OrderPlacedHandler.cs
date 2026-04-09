using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Services;
using Microsoft.Extensions.Logging;

namespace RVM.MenuNaMao.Application.Notifications;

public sealed class OrderPlacedHandler(IRabbitMqPublisher publisher, ILogger<OrderPlacedHandler> logger)
    : INotificationHandler<OrderPlacedNotification>
{
    public async Task Handle(OrderPlacedNotification notification, CancellationToken ct = default)
    {
        logger.LogInformation("Order {OrderId} placed for table {TableId}", notification.OrderId, notification.TableId);
        await publisher.PublishAsync("orders.placed", notification, ct);
    }
}
