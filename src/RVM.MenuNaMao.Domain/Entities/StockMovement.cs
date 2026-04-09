using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid StockItemId { get; set; }
    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StockItem StockItem { get; set; } = null!;
}
