using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Domain.Entities;

public class Table
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid RestaurantId { get; set; }
    public int Number { get; set; }
    public string QrCodeToken { get; set; } = string.Empty;
    public TableStatus Status { get; set; } = TableStatus.Available;

    public Restaurant Restaurant { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
}
