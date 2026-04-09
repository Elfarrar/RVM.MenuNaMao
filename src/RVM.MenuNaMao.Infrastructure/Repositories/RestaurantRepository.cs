using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class RestaurantRepository(MenuNaMaoDbContext db) : IRestaurantRepository
{
    public async Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Restaurants.FindAsync([id], ct);

    public async Task<Restaurant?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await db.Restaurants.FirstOrDefaultAsync(r => r.Slug == slug, ct);

    public async Task<List<Restaurant>> GetAllAsync(CancellationToken ct = default)
        => await db.Restaurants.OrderBy(r => r.Name).ToListAsync(ct);

    public async Task AddAsync(Restaurant restaurant, CancellationToken ct = default)
    {
        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Restaurant restaurant, CancellationToken ct = default)
    {
        db.Restaurants.Update(restaurant);
        await db.SaveChangesAsync(ct);
    }
}
