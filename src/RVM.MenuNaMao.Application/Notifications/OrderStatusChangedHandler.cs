using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Services;
using Microsoft.Extensions.Logging;

namespace RVM.MenuNaMao.Application.Notifications;

public sealed class OrderStatusChangedHandler(IRabbitMqPublisher publisher, ILogger<OrderStatusChangedHandler> logger)
    : INotificationHandler<OrderStatusChangedNotification>
{
    public async Task Handle(OrderStatusChangedNotification notification, CancellationToken ct = default)
    {
        logger.LogInformation("Order {OrderId} status changed from {Old} to {New}",
            notification.OrderId, notification.OldStatus, notification.NewStatus);
        await publisher.PublishAsync("orders.status-changed", notification, ct);
    }
}
