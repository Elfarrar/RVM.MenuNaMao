using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class OrderRepository(MenuNaMaoDbContext db) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<List<Order>> GetByRestaurantIdAsync(Guid restaurantId, OrderStatus? status = null, CancellationToken ct = default)
    {
        var query = db.Orders
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Include(o => o.Table)
            .Where(o => o.RestaurantId == restaurantId);

        if (status.HasValue) query = query.Where(o => o.Status == status.Value);

        return await query.OrderByDescending(o => o.CreatedAt).ToListAsync(ct);
    }

    public async Task<List<Order>> GetByTableIdAsync(Guid tableId, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Include(o => o.Table)
            .Where(o => o.TableId == tableId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Update(order);
        await db.SaveChangesAsync(ct);
    }
}
