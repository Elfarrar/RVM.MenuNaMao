namespace RVM.MenuNaMao.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? LogoUrl { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Table> Tables { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
    public List<StockItem> StockItems { get; set; } = [];
}
