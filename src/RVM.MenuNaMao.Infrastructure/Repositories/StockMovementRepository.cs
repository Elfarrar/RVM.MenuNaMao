using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class StockMovementRepository(MenuNaMaoDbContext db) : IStockMovementRepository
{
    public async Task<List<StockMovement>> GetByStockItemIdAsync(Guid stockItemId, CancellationToken ct = default)
        => await db.StockMovements
            .Where(m => m.StockItemId == stockItemId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(StockMovement movement, CancellationToken ct = default)
    {
        db.StockMovements.Add(movement);
        await db.SaveChangesAsync(ct);
    }
}
