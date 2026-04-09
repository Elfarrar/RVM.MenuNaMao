using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

    public Order Order { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}
