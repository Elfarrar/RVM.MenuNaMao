using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Order>> GetByRestaurantIdAsync(Guid restaurantId, OrderStatus? status = null, CancellationToken ct = default);
    Task<List<Order>> GetByTableIdAsync(Guid tableId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}
