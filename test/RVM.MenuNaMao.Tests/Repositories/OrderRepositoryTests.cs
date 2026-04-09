using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;

namespace RVM.MenuNaMao.Tests.Repositories;

public class OrderRepositoryTests
{
    [Fact]
    public async Task AddAsync_And_GetByIdAsync_RoundTrips()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "R1", Slug = "r1" };
        db.Restaurants.Add(restaurant);
        var table = new Table { RestaurantId = restaurant.Id, Number = 1, QrCodeToken = "abc" };
        db.Tables.Add(table);
        var category = new Category { RestaurantId = restaurant.Id, Name = "Cat", DisplayOrder = 1 };
        db.Categories.Add(category);
        var menuItem = new MenuItem { CategoryId = category.Id, Name = "Item", Price = 10 };
        db.MenuItems.Add(menuItem);
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        var order = new Order
        {
            RestaurantId = restaurant.Id,
            TableId = table.Id,
            TotalAmount = 20,
            Items = [new OrderItem { MenuItemId = menuItem.Id, Quantity = 2, UnitPrice = 10 }]
        };

        await repo.AddAsync(order);

        var found = await repo.GetByIdAsync(order.Id);
        Assert.NotNull(found);
        Assert.Equal(20, found.TotalAmount);
        Assert.Single(found.Items);
    }

    [Fact]
    public async Task GetByRestaurantIdAsync_FiltersStatus()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "R2", Slug = "r2" };
        db.Restaurants.Add(restaurant);
        var table = new Table { RestaurantId = restaurant.Id, Number = 1, QrCodeToken = "def" };
        db.Tables.Add(table);

        db.Orders.AddRange(
            new Order { RestaurantId = restaurant.Id, TableId = table.Id, Status = OrderStatus.Pending },
            new Order { RestaurantId = restaurant.Id, TableId = table.Id, Status = OrderStatus.Preparing },
            new Order { RestaurantId = restaurant.Id, TableId = table.Id, Status = OrderStatus.Pending }
        );
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        var all = await repo.GetByRestaurantIdAsync(restaurant.Id);
        Assert.Equal(3, all.Count);

        var pending = await repo.GetByRestaurantIdAsync(restaurant.Id, OrderStatus.Pending);
        Assert.Equal(2, pending.Count);
    }
}
