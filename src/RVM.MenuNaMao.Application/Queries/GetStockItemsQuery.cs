using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetStockItemsQuery(Guid RestaurantId) : IRequest<List<StockItemDto>>;

public sealed class GetStockItemsHandler(IStockItemRepository repo)
    : IRequestHandler<GetStockItemsQuery, List<StockItemDto>>
{
    public async Task<List<StockItemDto>> Handle(GetStockItemsQuery request, CancellationToken ct = default)
    {
        var items = await repo.GetByRestaurantIdAsync(request.RestaurantId, ct);

        return items.Select(i => new StockItemDto(
            i.Id, i.RestaurantId, i.Name, i.Unit,
            i.Quantity, i.MinQuantity, i.LastRestockedAt)).ToList();
    }
}
