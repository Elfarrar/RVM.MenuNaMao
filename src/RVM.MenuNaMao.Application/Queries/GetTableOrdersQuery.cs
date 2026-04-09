using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetTableOrdersQuery(Guid TableId) : IRequest<List<OrderDto>>;

public sealed class GetTableOrdersHandler(IOrderRepository repo) : IRequestHandler<GetTableOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetTableOrdersQuery request, CancellationToken ct = default)
    {
        var orders = await repo.GetByTableIdAsync(request.TableId, ct);

        return orders.Select(o => new OrderDto(
            o.Id, o.RestaurantId, o.TableId,
            o.Table?.Number ?? 0, o.CustomerName,
            o.Status, o.CreatedAt, o.TotalAmount,
            o.Items.Select(i => new OrderItemDto(
                i.Id, i.MenuItemId, i.MenuItem?.Name ?? "", i.Quantity,
                i.UnitPrice, i.Notes, i.Status)).ToList())).ToList();
    }
}
