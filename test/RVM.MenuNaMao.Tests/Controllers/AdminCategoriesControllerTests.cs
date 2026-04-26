using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminCategoriesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly AdminCategoriesController _controller;

    public AdminCategoriesControllerTests()
    {
        _controller = new AdminCategoriesController(_mediator.Object, _categoryRepo.Object);
    }

    private static Category MakeCategory(string name = "Pizzas", int order = 1)
        => new() { Name = name, DisplayOrder = order };

    private static CategoryDto MakeCategoryDto(string name = "Pizzas")
        => new(Guid.NewGuid(), Guid.NewGuid(), name, 1, true);

    [Fact]
    public async Task GetCategories_ReturnsCategories()
    {
        var restaurantId = Guid.NewGuid();
        var categories = new List<Category> { MakeCategory("Pizzas"), MakeCategory("Drinks", 2) };
        _categoryRepo.Setup(r => r.GetByRestaurantIdAsync(restaurantId, false, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(categories);

        var result = await _controller.GetCategories(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<Category>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetCategories_EmptyList_ReturnsEmpty()
    {
        var restaurantId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.GetByRestaurantIdAsync(restaurantId, false, It.IsAny<CancellationToken>()))
                     .ReturnsAsync([]);

        var result = await _controller.GetCategories(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<Category>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        var restaurantId = Guid.NewGuid();
        var request = new CreateCategoryRequest("Sobremesas", 3);
        var dto = MakeCategoryDto("Sobremesas");
        _mediator.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dto);

        var result = await _controller.Create(restaurantId, request);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Create_CallsMediatorWithCorrectArgs()
    {
        var restaurantId = Guid.NewGuid();
        var request = new CreateCategoryRequest("Bebidas", 2);
        _mediator.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeCategoryDto("Bebidas"));

        await _controller.Create(restaurantId, request);

        _mediator.Verify(m => m.Send(
            It.Is<CreateCategoryCommand>(c => c.RestaurantId == restaurantId && c.Name == "Bebidas" && c.DisplayOrder == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ExistingCategory_ReturnsNoContent()
    {
        var restaurantId = Guid.NewGuid();
        var category = MakeCategory();
        _categoryRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _categoryRepo.Setup(r => r.UpdateAsync(category, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _controller.Update(restaurantId, category.Id, new UpdateCategoryRequest("Novo Nome", 5, false));

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Novo Nome", category.Name);
        Assert.Equal(5, category.DisplayOrder);
        Assert.False(category.Active);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var restaurantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        _categoryRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var result = await _controller.Update(restaurantId, id, new UpdateCategoryRequest("X", 1, true));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_CallsDeleteAsync()
    {
        var restaurantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        _categoryRepo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _controller.Delete(restaurantId, id);

        Assert.IsType<NoContentResult>(result);
        _categoryRepo.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
