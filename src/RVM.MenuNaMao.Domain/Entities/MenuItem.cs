namespace RVM.MenuNaMao.Domain.Entities;

public class MenuItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool Available { get; set; } = true;
    public int PreparationTimeMinutes { get; set; }

    public Category Category { get; set; } = null!;
}
