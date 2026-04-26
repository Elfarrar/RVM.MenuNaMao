using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Tests.Controllers;

public class TablesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly TablesController _controller;

    public TablesControllerTests()
    {
        _controller = new TablesController(_mediator.Object);
    }

    private static TableDto MakeTableDto(string token = "abc123")
        => new(Guid.NewGuid(), Guid.NewGuid(), 1, token, TableStatus.Available);

    [Fact]
    public async Task ResolveTable_ValidToken_ReturnsOk()
    {
        var tableDto = MakeTableDto("mytoken");
        _mediator.Setup(m => m.Send(It.Is<ResolveTableQuery>(q => q.QrCodeToken == "mytoken"), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(tableDto);

        var result = await _controller.ResolveTable("mytoken");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tableDto, ok.Value);
    }

    [Fact]
    public async Task ResolveTable_PassesTokenToQuery()
    {
        _mediator.Setup(m => m.Send(It.IsAny<ResolveTableQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeTableDto());

        await _controller.ResolveTable("tok-xyz");

        _mediator.Verify(m => m.Send(
            It.Is<ResolveTableQuery>(q => q.QrCodeToken == "tok-xyz"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResolveTable_ReturnsCorrectTableNumber()
    {
        var tableDto = new TableDto(Guid.NewGuid(), Guid.NewGuid(), 7, "tok7", TableStatus.Available);
        _mediator.Setup(m => m.Send(It.IsAny<ResolveTableQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(tableDto);

        var result = await _controller.ResolveTable("tok7");

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<TableDto>(ok.Value);
        Assert.Equal(7, dto.Number);
    }
}
