using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _controller = new OrdersController(_mediator.Object);
    }

    private static OrderDto MakeOrderDto(OrderStatus status = OrderStatus.Pending)
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, "Cliente", status, DateTime.UtcNow, 50m, []);

    [Fact]
    public async Task PlaceOrder_ReturnsCreated()
    {
        var orderDto = MakeOrderDto();
        var command = new PlaceOrderCommand(Guid.NewGuid(), "Maria", []);
        _mediator.Setup(m => m.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(orderDto);

        var result = await _controller.PlaceOrder(command);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(orderDto, created.Value);
    }

    [Fact]
    public async Task PlaceOrder_CallsMediatorSend()
    {
        var command = new PlaceOrderCommand(Guid.NewGuid(), "Joao", []);
        _mediator.Setup(m => m.Send(It.IsAny<PlaceOrderCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeOrderDto());

        await _controller.PlaceOrder(command);

        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrder_ExistingOrder_ReturnsOk()
    {
        var orderId = Guid.NewGuid();
        var orderDto = MakeOrderDto();
        _mediator.Setup(m => m.Send(It.Is<GetOrderQuery>(q => q.OrderId == orderId), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(orderDto);

        var result = await _controller.GetOrder(orderId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orderDto, ok.Value);
    }

    [Fact]
    public async Task GetOrder_PassesIdToQuery()
    {
        var orderId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetOrderQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeOrderDto());

        await _controller.GetOrder(orderId);

        _mediator.Verify(m => m.Send(It.Is<GetOrderQuery>(q => q.OrderId == orderId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
