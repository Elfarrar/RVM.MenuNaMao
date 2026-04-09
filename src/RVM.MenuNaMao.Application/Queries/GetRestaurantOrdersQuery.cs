using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetRestaurantOrdersQuery(Guid RestaurantId, OrderStatus? Status = null) : IRequest<List<OrderDto>>;

public sealed class GetRestaurantOrdersHandler(IOrderRepository repo)
    : IRequestHandler<GetRestaurantOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetRestaurantOrdersQuery request, CancellationToken ct = default)
    {
        var orders = await repo.GetByRestaurantIdAsync(request.RestaurantId, request.Status, ct);

        return orders.Select(o => new OrderDto(
            o.Id, o.RestaurantId, o.TableId,
            o.Table?.Number ?? 0, o.CustomerName,
            o.Status, o.CreatedAt, o.TotalAmount,
            o.Items.Select(i => new OrderItemDto(
                i.Id, i.MenuItemId, i.MenuItem?.Name ?? "", i.Quantity,
                i.UnitPrice, i.Notes, i.Status)).ToList())).ToList();
    }
}
