using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminMenuItemsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IMenuItemRepository> _menuItemRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly AdminMenuItemsController _controller;

    public AdminMenuItemsControllerTests()
    {
        _controller = new AdminMenuItemsController(_mediator.Object, _menuItemRepo.Object, _categoryRepo.Object);
    }

    private static MenuItem MakeItem(string name = "Pizza Margherita", decimal price = 35m)
        => new() { Name = name, Price = price };

    private static MenuItemDto MakeItemDto(string name = "Pizza Margherita")
        => new(Guid.NewGuid(), Guid.NewGuid(), name, null, 35m, null, true, 15);

    [Fact]
    public async Task GetMenuItems_AggregatesItemsFromCategories()
    {
        var restaurantId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        var category = new Category { Name = "Pizzas", DisplayOrder = 1 };
        var items = new List<MenuItem> { MakeItem("Pizza A"), MakeItem("Pizza B") };

        _categoryRepo.Setup(r => r.GetByRestaurantIdAsync(restaurantId, false, It.IsAny<CancellationToken>()))
                     .ReturnsAsync([category]);
        _menuItemRepo.Setup(r => r.GetByCategoryIdAsync(category.Id, false, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(items);

        var result = await _controller.GetMenuItems(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<object>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetMenuItems_NoCategories_ReturnsEmptyList()
    {
        var restaurantId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.GetByRestaurantIdAsync(restaurantId, false, It.IsAny<CancellationToken>()))
                     .ReturnsAsync([]);

        var result = await _controller.GetMenuItems(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<object>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreated()
    {
        var restaurantId = Guid.NewGuid();
        var command = new CreateMenuItemCommand(Guid.NewGuid(), "Calzone", null, 28m, null, 20);
        var dto = MakeItemDto("Calzone");
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Create(restaurantId, command);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Create_CallsMediatorSend()
    {
        var restaurantId = Guid.NewGuid();
        var command = new CreateMenuItemCommand(Guid.NewGuid(), "Bruschetta", "Tomate e alho", 12m, null, 10);
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(MakeItemDto());

        await _controller.Create(restaurantId, command);

        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ExistingItem_ReturnsNoContent()
    {
        var restaurantId = Guid.NewGuid();
        var item = MakeItem();
        _menuItemRepo.Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);
        _menuItemRepo.Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new UpdateMenuItemRequest("Pizza Pepperoni", null, 40m, null, true, 20);
        var result = await _controller.Update(restaurantId, item.Id, request);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Pizza Pepperoni", item.Name);
        Assert.Equal(40m, item.Price);
        Assert.True(item.Available);
        Assert.Equal(20, item.PreparationTimeMinutes);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var restaurantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        _menuItemRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((MenuItem?)null);

        var result = await _controller.Update(restaurantId, id,
            new UpdateMenuItemRequest("X", null, 10m, null, true, 5));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_CallsDeleteAsync()
    {
        var restaurantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        _menuItemRepo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _controller.Delete(restaurantId, id);

        Assert.IsType<NoContentResult>(result);
        _menuItemRepo.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
