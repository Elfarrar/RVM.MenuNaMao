using RVM.MenuNaMao.Application.Mediator;

namespace RVM.MenuNaMao.Application.Notifications;

public record StockLowNotification(Guid StockItemId, string Name, decimal Quantity, decimal MinQuantity) : INotification;
