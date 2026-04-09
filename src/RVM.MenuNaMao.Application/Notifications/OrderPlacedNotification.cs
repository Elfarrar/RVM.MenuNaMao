using RVM.MenuNaMao.Application.Mediator;

namespace RVM.MenuNaMao.Application.Notifications;

public record OrderPlacedNotification(Guid OrderId, Guid RestaurantId, Guid TableId) : INotification;
