using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Category>> GetByRestaurantIdAsync(Guid restaurantId, bool activeOnly = false, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    Task UpdateAsync(Category category, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
