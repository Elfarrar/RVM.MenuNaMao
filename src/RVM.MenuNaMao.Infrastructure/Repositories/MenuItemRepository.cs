using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class MenuItemRepository(MenuNaMaoDbContext db) : IMenuItemRepository
{
    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.MenuItems.FindAsync([id], ct);

    public async Task<List<MenuItem>> GetByCategoryIdAsync(Guid categoryId, bool availableOnly = false, CancellationToken ct = default)
    {
        var query = db.MenuItems.Where(m => m.CategoryId == categoryId);
        if (availableOnly) query = query.Where(m => m.Available);
        return await query.OrderBy(m => m.Name).ToListAsync(ct);
    }

    public async Task AddAsync(MenuItem item, CancellationToken ct = default)
    {
        db.MenuItems.Add(item);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MenuItem item, CancellationToken ct = default)
    {
        db.MenuItems.Update(item);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.MenuItems.FindAsync([id], ct);
        if (item is not null)
        {
            db.MenuItems.Remove(item);
            await db.SaveChangesAsync(ct);
        }
    }
}
