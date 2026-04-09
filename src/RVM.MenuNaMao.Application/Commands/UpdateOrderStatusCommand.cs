using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<Unit>;

public sealed class UpdateOrderStatusHandler(IOrderRepository repo, IMediator mediator)
    : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken ct = default)
    {
        var order = await repo.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found");

        var oldStatus = order.Status;
        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(order, ct);
        await mediator.Publish(new OrderStatusChangedNotification(order.Id, oldStatus, request.NewStatus), ct);

        return Unit.Value;
    }
}
