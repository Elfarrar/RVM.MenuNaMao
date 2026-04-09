using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Table?> GetByQrCodeTokenAsync(string token, CancellationToken ct = default);
    Task<List<Table>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default);
    Task AddAsync(Table table, CancellationToken ct = default);
    Task UpdateAsync(Table table, CancellationToken ct = default);
}
