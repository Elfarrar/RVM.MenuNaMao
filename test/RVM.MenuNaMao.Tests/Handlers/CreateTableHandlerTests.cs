using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;

namespace RVM.MenuNaMao.Tests.Handlers;

public class CreateTableHandlerTests
{
    [Fact]
    public async Task Handle_CreatesTable_GeneratesQrCodeToken()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Test", Slug = "test" };
        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync();

        var repo = new TableRepository(db);
        var handler = new CreateTableHandler(repo);

        var result = await handler.Handle(new CreateTableCommand(restaurant.Id, 5));

        Assert.Equal(5, result.Number);
        Assert.Equal(restaurant.Id, result.RestaurantId);
        Assert.Equal(TableStatus.Available, result.Status);
        Assert.Equal(32, result.QrCodeToken.Length); // Guid without hyphens = 32 chars
    }
}
