using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Entities;

public class StockItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public StockUnit Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal MinQuantity { get; set; }
    public DateTime? LastRestockedAt { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    public List<StockMovement> Movements { get; set; } = [];
}
