using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;
using Moq;

namespace RVM.MenuNaMao.Tests.Handlers;

public class PlaceOrderHandlerTests
{
    [Fact]
    public async Task Handle_CreatesOrder_CalculatesTotal_PublishesNotification()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Test", Slug = "test" };
        db.Restaurants.Add(restaurant);

        var table = new Table { RestaurantId = restaurant.Id, Number = 1, QrCodeToken = "tok1" };
        db.Tables.Add(table);

        var category = new Category { RestaurantId = restaurant.Id, Name = "Pratos", DisplayOrder = 1 };
        db.Categories.Add(category);

        var item1 = new MenuItem { CategoryId = category.Id, Name = "Pizza", Price = 35.00m };
        var item2 = new MenuItem { CategoryId = category.Id, Name = "Suco", Price = 8.50m };
        db.MenuItems.AddRange(item1, item2);
        await db.SaveChangesAsync();

        var orderRepo = new OrderRepository(db);
        var tableRepo = new TableRepository(db);
        var menuItemRepo = new MenuItemRepository(db);
        var mediatorMock = new Mock<IMediator>();

        var handler = new PlaceOrderHandler(orderRepo, tableRepo, menuItemRepo, mediatorMock.Object);

        var command = new PlaceOrderCommand(table.Id, "João", [
            new PlaceOrderItemRequest(item1.Id, 2, null),
            new PlaceOrderItemRequest(item2.Id, 1, "Sem gelo")
        ]);

        var result = await handler.Handle(command);

        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Equal(78.50m, result.TotalAmount); // 2*35 + 1*8.50
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("João", result.CustomerName);

        mediatorMock.Verify(m => m.Publish(It.IsAny<OrderPlacedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
