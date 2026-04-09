using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class CategoryRepository(MenuNaMaoDbContext db) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Categories.FindAsync([id], ct);

    public async Task<List<Category>> GetByRestaurantIdAsync(Guid restaurantId, bool activeOnly = false, CancellationToken ct = default)
    {
        var query = db.Categories.Where(c => c.RestaurantId == restaurantId);
        if (activeOnly) query = query.Where(c => c.Active);
        return await query.OrderBy(c => c.DisplayOrder).ToListAsync(ct);
    }

    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        db.Categories.Update(category);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([id], ct);
        if (category is not null)
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync(ct);
        }
    }
}
