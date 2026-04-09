namespace RVM.MenuNaMao.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool Active { get; set; } = true;

    public Restaurant Restaurant { get; set; } = null!;
    public List<MenuItem> MenuItems { get; set; } = [];
}
