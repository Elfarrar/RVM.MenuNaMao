using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.Notifications;

public record OrderStatusChangedNotification(Guid OrderId, OrderStatus OldStatus, OrderStatus NewStatus) : INotification;
