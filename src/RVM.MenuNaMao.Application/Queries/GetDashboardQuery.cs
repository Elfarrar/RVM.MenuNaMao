using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetDashboardQuery(Guid RestaurantId) : IRequest<DashboardDto>;

public sealed class GetDashboardHandler(IOrderRepository orderRepo, IStockItemRepository stockRepo)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct = default)
    {
        var allOrders = await orderRepo.GetByRestaurantIdAsync(request.RestaurantId, ct: ct);
        var today = DateTime.UtcNow.Date;

        var pendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending);
        var preparingOrders = allOrders.Count(o => o.Status == OrderStatus.Preparing);
        var todayOrders = allOrders.Where(o => o.CreatedAt.Date == today).ToList();
        var todayRevenue = todayOrders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .Sum(o => o.TotalAmount);

        var stockItems = await stockRepo.GetByRestaurantIdAsync(request.RestaurantId, ct);
        var alerts = stockItems
            .Where(s => s.Quantity <= s.MinQuantity)
            .Select(s => new StockAlertDto(s.Id, s.Name, s.Quantity, s.MinQuantity))
            .ToList();

        return new DashboardDto(pendingOrders, preparingOrders, todayOrders.Count, todayRevenue, alerts);
    }
}
