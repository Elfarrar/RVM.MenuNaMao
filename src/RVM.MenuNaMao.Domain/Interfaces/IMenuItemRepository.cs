using RVM.MenuNaMao.Domain.Entities;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<MenuItem>> GetByCategoryIdAsync(Guid categoryId, bool availableOnly = false, CancellationToken ct = default);
    Task AddAsync(MenuItem item, CancellationToken ct = default);
    Task UpdateAsync(MenuItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
