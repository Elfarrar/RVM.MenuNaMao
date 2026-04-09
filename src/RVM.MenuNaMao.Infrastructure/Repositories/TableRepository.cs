using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;
using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Repositories;

public sealed class TableRepository(MenuNaMaoDbContext db) : ITableRepository
{
    public async Task<Table?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Tables.FindAsync([id], ct);

    public async Task<Table?> GetByQrCodeTokenAsync(string token, CancellationToken ct = default)
        => await db.Tables.FirstOrDefaultAsync(t => t.QrCodeToken == token, ct);

    public async Task<List<Table>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default)
        => await db.Tables.Where(t => t.RestaurantId == restaurantId).OrderBy(t => t.Number).ToListAsync(ct);

    public async Task AddAsync(Table table, CancellationToken ct = default)
    {
        db.Tables.Add(table);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Table table, CancellationToken ct = default)
    {
        db.Tables.Update(table);
        await db.SaveChangesAsync(ct);
    }
}
