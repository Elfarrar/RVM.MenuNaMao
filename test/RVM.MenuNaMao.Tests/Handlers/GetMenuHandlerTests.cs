using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;

namespace RVM.MenuNaMao.Tests.Handlers;

public class GetMenuHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsActiveCategoriesAndItems()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Pizzaria", Slug = "pizzaria" };
        db.Restaurants.Add(restaurant);

        var cat1 = new Category { RestaurantId = restaurant.Id, Name = "Pizzas", DisplayOrder = 1, Active = true };
        var cat2 = new Category { RestaurantId = restaurant.Id, Name = "Sobremesas", DisplayOrder = 2, Active = false };
        db.Categories.AddRange(cat1, cat2);

        var item1 = new MenuItem { CategoryId = cat1.Id, Name = "Margherita", Price = 40, Available = true };
        var item2 = new MenuItem { CategoryId = cat1.Id, Name = "Calabresa", Price = 45, Available = false };
        db.MenuItems.AddRange(item1, item2);
        await db.SaveChangesAsync();

        var restaurantRepo = new RestaurantRepository(db);
        var categoryRepo = new CategoryRepository(db);
        var menuItemRepo = new MenuItemRepository(db);
        var handler = new GetMenuHandler(restaurantRepo, categoryRepo, menuItemRepo);

        var result = await handler.Handle(new GetMenuQuery("pizzaria"));

        Assert.Equal("Pizzaria", result.Restaurant.Name);
        Assert.Single(result.Categories); // only active category
        Assert.Single(result.Categories[0].Items); // only available item
        Assert.Equal("Margherita", result.Categories[0].Items[0].Name);
    }
}
