using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;

namespace RVM.MenuNaMao.Tests.Controllers;

public class MenuControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly MenuController _controller;

    public MenuControllerTests()
    {
        _controller = new MenuController(_mediator.Object);
    }

    private static MenuDto MakeMenuDto(string restaurantName = "Pizzaria Roma", string slug = "pizzaria-roma")
    {
        var restaurant = new RestaurantDto(Guid.NewGuid(), restaurantName, slug, null, null, null, true, DateTime.UtcNow);
        return new MenuDto(restaurant, []);
    }

    [Fact]
    public async Task GetMenu_ExistingSlug_ReturnsOk()
    {
        var menuDto = MakeMenuDto();
        _mediator.Setup(m => m.Send(It.IsAny<GetMenuQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(menuDto);

        var result = await _controller.GetMenu("pizzaria-roma");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<MenuDto>(ok.Value);
    }

    [Fact]
    public async Task GetMenu_PassesSlugToQuery()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetMenuQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeMenuDto("Test", "test-slug"));

        await _controller.GetMenu("test-slug");

        _mediator.Verify(m => m.Send(
            It.Is<GetMenuQuery>(q => q.RestaurantSlug == "test-slug"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMenu_ReturnsMenuWithRestaurantInfo()
    {
        var menuDto = MakeMenuDto("Sushi House", "sushi-house");
        _mediator.Setup(m => m.Send(It.IsAny<GetMenuQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(menuDto);

        var result = await _controller.GetMenu("sushi-house");

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<MenuDto>(ok.Value);
        Assert.Equal("Sushi House", returned.Restaurant.Name);
    }
}
