using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<StockItem>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default);
    Task AddAsync(StockItem item, CancellationToken ct = default);
    Task UpdateAsync(StockItem item, CancellationToken ct = default);
}
