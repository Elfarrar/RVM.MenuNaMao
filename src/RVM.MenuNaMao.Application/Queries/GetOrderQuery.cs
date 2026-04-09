using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;

public sealed class GetOrderHandler(IOrderRepository repo) : IRequestHandler<GetOrderQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken ct = default)
    {
        var order = await repo.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found");

        var itemDtos = order.Items.Select(i => new OrderItemDto(
            i.Id, i.MenuItemId, i.MenuItem?.Name ?? "", i.Quantity,
            i.UnitPrice, i.Notes, i.Status)).ToList();

        return new OrderDto(
            order.Id, order.RestaurantId, order.TableId,
            order.Table?.Number ?? 0, order.CustomerName,
            order.Status, order.CreatedAt, order.TotalAmount, itemDtos);
    }
}
