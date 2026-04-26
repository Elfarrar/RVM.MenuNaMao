using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Application.Services;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminTablesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IQrCodeService> _qrCodeService = new();
    private readonly AdminTablesController _controller;

    public AdminTablesControllerTests()
    {
        _controller = new AdminTablesController(_mediator.Object, _qrCodeService.Object);

        // Set up a minimal HttpContext so Request.Scheme/Host work
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("localhost");
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private static TableDto MakeTableDto(int number = 1, string token = "tok123")
        => new(Guid.NewGuid(), Guid.NewGuid(), number, token, TableStatus.Available);

    [Fact]
    public async Task GetTables_ReturnsTables()
    {
        var restaurantId = Guid.NewGuid();
        var tables = new List<TableDto> { MakeTableDto(1), MakeTableDto(2) };
        _mediator.Setup(m => m.Send(It.IsAny<GetTablesQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(tables);

        var result = await _controller.GetTables(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<TableDto>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetTables_PassesRestaurantIdToQuery()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetTablesQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        await _controller.GetTables(restaurantId);

        _mediator.Verify(m => m.Send(
            It.Is<GetTablesQuery>(q => q.RestaurantId == restaurantId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTable_ReturnsCreated()
    {
        var restaurantId = Guid.NewGuid();
        var tableDto = MakeTableDto(3);
        _mediator.Setup(m => m.Send(It.IsAny<CreateTableCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(tableDto);

        var result = await _controller.CreateTable(restaurantId, new CreateTableRequest(3));

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal(tableDto, created.Value);
    }

    [Fact]
    public async Task CreateTable_CallsMediatorWithCorrectArgs()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<CreateTableCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeTableDto());

        await _controller.CreateTable(restaurantId, new CreateTableRequest(5));

        _mediator.Verify(m => m.Send(
            It.Is<CreateTableCommand>(c => c.RestaurantId == restaurantId && c.Number == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetQrCode_TableNotFound_Returns404()
    {
        var restaurantId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetTablesQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        var result = await _controller.GetQrCode(restaurantId, tableId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetQrCode_TableFound_ReturnsPngFile()
    {
        var restaurantId = Guid.NewGuid();
        var tableDto = MakeTableDto(2, "tok-for-qr");
        _mediator.Setup(m => m.Send(It.IsAny<GetTablesQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([tableDto]);
        _qrCodeService.Setup(q => q.GenerateQrCodePng(It.IsAny<string>()))
                      .Returns([0x89, 0x50, 0x4E, 0x47]); // PNG magic bytes

        var result = await _controller.GetQrCode(restaurantId, tableDto.Id);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", file.ContentType);
    }
}
