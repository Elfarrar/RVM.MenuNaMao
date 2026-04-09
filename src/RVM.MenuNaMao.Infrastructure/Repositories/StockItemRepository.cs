using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class StockItemRepository(MenuNaMaoDbContext db) : IStockItemRepository
{
    public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.StockItems.FindAsync([id], ct);

    public async Task<List<StockItem>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default)
        => await db.StockItems.Where(s => s.RestaurantId == restaurantId).OrderBy(s => s.Name).ToListAsync(ct);

    public async Task AddAsync(StockItem item, CancellationToken ct = default)
    {
        db.StockItems.Add(item);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(StockItem item, CancellationToken ct = default)
    {
        db.StockItems.Update(item);
        await db.SaveChangesAsync(ct);
    }
}
