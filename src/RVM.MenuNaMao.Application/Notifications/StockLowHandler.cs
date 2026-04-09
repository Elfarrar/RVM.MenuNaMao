using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Services;
using Microsoft.Extensions.Logging;

namespace RVM.MenuNaMao.Application.Notifications;

public sealed class StockLowHandler(IRabbitMqPublisher publisher, ILogger<StockLowHandler> logger)
    : INotificationHandler<StockLowNotification>
{
    public async Task Handle(StockLowNotification notification, CancellationToken ct = default)
    {
        logger.LogWarning("Stock low alert: {Name} has {Qty}/{Min}",
            notification.Name, notification.Quantity, notification.MinQuantity);
        await publisher.PublishAsync("stock.low-alert", notification, ct);
    }
}
