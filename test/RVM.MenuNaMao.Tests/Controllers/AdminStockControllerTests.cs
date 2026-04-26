using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminStockControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IStockItemRepository> _stockRepo = new();
    private readonly AdminStockController _controller;

    public AdminStockControllerTests()
    {
        _controller = new AdminStockController(_mediator.Object, _stockRepo.Object);
    }

    private static StockItemDto MakeStockItemDto(string name = "Farinha")
        => new(Guid.NewGuid(), Guid.NewGuid(), name, StockUnit.Kg, 10m, 2m, null);

    [Fact]
    public async Task GetStockItems_ReturnsOk()
    {
        var restaurantId = Guid.NewGuid();
        var items = new List<StockItemDto> { MakeStockItemDto("Farinha"), MakeStockItemDto("Queijo") };
        _mediator.Setup(m => m.Send(It.IsAny<GetStockItemsQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(items);

        var result = await _controller.GetStockItems(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<StockItemDto>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetStockItems_PassesRestaurantIdToQuery()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetStockItemsQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        await _controller.GetStockItems(restaurantId);

        _mediator.Verify(m => m.Send(
            It.Is<GetStockItemsQuery>(q => q.RestaurantId == restaurantId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateStockItem_ReturnsCreated()
    {
        var restaurantId = Guid.NewGuid();
        var request = new CreateStockItemRequest("Tomate", StockUnit.Kg, 20m, 3m);
        _stockRepo.Setup(r => r.AddAsync(It.IsAny<StockItem>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        var result = await _controller.CreateStockItem(restaurantId, request);

        var created = Assert.IsType<CreatedResult>(result);
        var dto = Assert.IsType<StockItemDto>(created.Value);
        Assert.Equal("Tomate", dto.Name);
        Assert.Equal(20m, dto.Quantity);
        Assert.Equal(3m, dto.MinQuantity);
    }

    [Fact]
    public async Task CreateStockItem_CallsAddAsync()
    {
        var restaurantId = Guid.NewGuid();
        var request = new CreateStockItemRequest("Carne", StockUnit.Kg, 50m, 10m);
        _stockRepo.Setup(r => r.AddAsync(It.IsAny<StockItem>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _controller.CreateStockItem(restaurantId, request);

        _stockRepo.Verify(r => r.AddAsync(
            It.Is<StockItem>(s => s.Name == "Carne" && s.RestaurantId == restaurantId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AdminStockMovementsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IStockMovementRepository> _movementRepo = new();
    private readonly AdminStockMovementsController _controller;

    public AdminStockMovementsControllerTests()
    {
        _controller = new AdminStockMovementsController(_mediator.Object, _movementRepo.Object);
    }

    private static StockMovement MakeMovement(StockMovementType type = StockMovementType.In)
        => new()
        {
            StockItemId = Guid.NewGuid(),
            Type = type,
            Quantity = 5m,
            Reason = "Restock"
        };

    [Fact]
    public async Task GetMovements_ReturnsOk()
    {
        var stockItemId = Guid.NewGuid();
        var movements = new List<StockMovement> { MakeMovement(), MakeMovement(StockMovementType.Out) };
        _movementRepo.Setup(r => r.GetByStockItemIdAsync(stockItemId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(movements);

        var result = await _controller.GetMovements(stockItemId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task AddMovement_ReturnsCreated()
    {
        var stockItemId = Guid.NewGuid();
        var dto = new StockMovementDto(Guid.NewGuid(), stockItemId, StockMovementType.In, 10m, "Entrada", DateTime.UtcNow);
        _mediator.Setup(m => m.Send(It.IsAny<AddStockMovementCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dto);

        var result = await _controller.AddMovement(stockItemId, new AddMovementRequest(StockMovementType.In, 10m, "Entrada"));

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task AddMovement_CallsMediatorWithCorrectArgs()
    {
        var stockItemId = Guid.NewGuid();
        var dto = new StockMovementDto(Guid.NewGuid(), stockItemId, StockMovementType.Out, 3m, null, DateTime.UtcNow);
        _mediator.Setup(m => m.Send(It.IsAny<AddStockMovementCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dto);

        await _controller.AddMovement(stockItemId, new AddMovementRequest(StockMovementType.Out, 3m, null));

        _mediator.Verify(m => m.Send(
            It.Is<AddStockMovementCommand>(c => c.StockItemId == stockItemId && c.Type == StockMovementType.Out && c.Quantity == 3m),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
