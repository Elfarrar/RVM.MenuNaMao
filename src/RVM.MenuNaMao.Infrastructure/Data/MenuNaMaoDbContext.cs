using RVM.MenuNaMao.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Infrastructure.Data;

public class MenuNaMaoDbContext(DbContextOptions<MenuNaMaoDbContext> options) : DbContext(options)
{
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MenuNaMaoDbContext).Assembly);
    }
}
