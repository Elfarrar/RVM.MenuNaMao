using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid RestaurantId { get; set; }
    public Guid TableId { get; set; }
    public string? CustomerName { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public decimal TotalAmount { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    public Table Table { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = [];
}
