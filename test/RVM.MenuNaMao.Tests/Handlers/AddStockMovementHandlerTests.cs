using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;
using Moq;

namespace RVM.MenuNaMao.Tests.Handlers;

public class AddStockMovementHandlerTests
{
    [Fact]
    public async Task Handle_InMovement_IncreasesQuantity()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Test", Slug = "test" };
        db.Restaurants.Add(restaurant);

        var stockItem = new StockItem { RestaurantId = restaurant.Id, Name = "Arroz", Unit = StockUnit.Kg, Quantity = 10, MinQuantity = 5 };
        db.StockItems.Add(stockItem);
        await db.SaveChangesAsync();

        var stockRepo = new StockItemRepository(db);
        var movementRepo = new StockMovementRepository(db);
        var mediatorMock = new Mock<IMediator>();
        var handler = new AddStockMovementHandler(stockRepo, movementRepo, mediatorMock.Object);

        var result = await handler.Handle(new AddStockMovementCommand(stockItem.Id, StockMovementType.In, 5, "Compra"));

        Assert.Equal(StockMovementType.In, result.Type);
        var updated = await stockRepo.GetByIdAsync(stockItem.Id);
        Assert.Equal(15, updated!.Quantity);

        mediatorMock.Verify(m => m.Publish(It.IsAny<StockLowNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_OutMovement_TriggersLowAlert_WhenBelowMinimum()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Test", Slug = "test" };
        db.Restaurants.Add(restaurant);

        var stockItem = new StockItem { RestaurantId = restaurant.Id, Name = "Feijão", Unit = StockUnit.Kg, Quantity = 6, MinQuantity = 5 };
        db.StockItems.Add(stockItem);
        await db.SaveChangesAsync();

        var stockRepo = new StockItemRepository(db);
        var movementRepo = new StockMovementRepository(db);
        var mediatorMock = new Mock<IMediator>();
        var handler = new AddStockMovementHandler(stockRepo, movementRepo, mediatorMock.Object);

        await handler.Handle(new AddStockMovementCommand(stockItem.Id, StockMovementType.Out, 2, "Consumo"));

        var updated = await stockRepo.GetByIdAsync(stockItem.Id);
        Assert.Equal(4, updated!.Quantity);

        mediatorMock.Verify(m => m.Publish(
            It.Is<StockLowNotification>(n => n.Name == "Feijão"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
