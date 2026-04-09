using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Infrastructure.Repositories;
using RVM.MenuNaMao.Tests.Helpers;
using Moq;

namespace RVM.MenuNaMao.Tests.Handlers;

public class UpdateOrderStatusHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesStatus_PublishesNotification()
    {
        using var db = TestDbContext.Create();

        var restaurant = new Restaurant { Name = "Test", Slug = "test" };
        db.Restaurants.Add(restaurant);

        var table = new Table { RestaurantId = restaurant.Id, Number = 1, QrCodeToken = "tok2" };
        db.Tables.Add(table);

        var order = new Order { RestaurantId = restaurant.Id, TableId = table.Id, Status = OrderStatus.Pending };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateOrderStatusHandler(repo, mediatorMock.Object);

        await handler.Handle(new UpdateOrderStatusCommand(order.Id, OrderStatus.Preparing));

        var updated = await repo.GetByIdAsync(order.Id);
        Assert.Equal(OrderStatus.Preparing, updated!.Status);
        Assert.NotNull(updated.UpdatedAt);

        mediatorMock.Verify(m => m.Publish(
            It.Is<OrderStatusChangedNotification>(n => n.OldStatus == OrderStatus.Pending && n.NewStatus == OrderStatus.Preparing),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
