using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Restaurant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<Restaurant>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Restaurant restaurant, CancellationToken ct = default);
    Task UpdateAsync(Restaurant restaurant, CancellationToken ct = default);
}
