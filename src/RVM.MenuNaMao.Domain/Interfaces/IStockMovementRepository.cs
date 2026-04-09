using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface IStockMovementRepository
{
    Task<List<StockMovement>> GetByStockItemIdAsync(Guid stockItemId, CancellationToken ct = default);
    Task AddAsync(StockMovement movement, CancellationToken ct = default);
}
