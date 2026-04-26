using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminOrdersControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly AdminOrdersController _controller;

    public AdminOrdersControllerTests()
    {
        _controller = new AdminOrdersController(_mediator.Object);
    }

    private static OrderDto MakeOrderDto(OrderStatus status = OrderStatus.Pending)
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, "Cliente", status, DateTime.UtcNow, 100m, []);

    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        var restaurantId = Guid.NewGuid();
        var orders = new List<OrderDto> { MakeOrderDto(), MakeOrderDto(OrderStatus.Preparing) };
        _mediator.Setup(m => m.Send(It.IsAny<GetRestaurantOrdersQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(orders);

        var result = await _controller.GetOrders(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<OrderDto>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetOrders_WithStatusFilter_PassesFilterToQuery()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetRestaurantOrdersQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([MakeOrderDto(OrderStatus.Pending)]);

        await _controller.GetOrders(restaurantId, OrderStatus.Pending);

        _mediator.Verify(m => m.Send(
            It.Is<GetRestaurantOrdersQuery>(q => q.RestaurantId == restaurantId && q.Status == OrderStatus.Pending),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrders_NoStatusFilter_PassesNullStatus()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetRestaurantOrdersQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        await _controller.GetOrders(restaurantId, null);

        _mediator.Verify(m => m.Send(
            It.Is<GetRestaurantOrdersQuery>(q => q.Status == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_CallsMediatorAndReturnsNoContent()
    {
        var orderId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Unit.Value);

        var result = await _controller.UpdateStatus(orderId, new UpdateStatusRequest(OrderStatus.Preparing));

        Assert.IsType<NoContentResult>(result);
        _mediator.Verify(m => m.Send(
            It.Is<UpdateOrderStatusCommand>(c => c.OrderId == orderId && c.NewStatus == OrderStatus.Preparing),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_CancelledStatus_PassesCancelledToCommand()
    {
        var orderId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Unit.Value);

        await _controller.UpdateStatus(orderId, new UpdateStatusRequest(OrderStatus.Cancelled));

        _mediator.Verify(m => m.Send(
            It.Is<UpdateOrderStatusCommand>(c => c.NewStatus == OrderStatus.Cancelled),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
