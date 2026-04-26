using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminRestaurantsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IRestaurantRepository> _repo = new();
    private readonly AdminRestaurantsController _controller;

    public AdminRestaurantsControllerTests()
    {
        _controller = new AdminRestaurantsController(_mediator.Object, _repo.Object);
    }

    private static Restaurant MakeRestaurant(string name = "Pizzaria Roma")
        => new() { Name = name, Slug = name.ToLower().Replace(" ", "-") };

    private static RestaurantDto MakeRestaurantDto(string name = "Pizzaria Roma", string slug = "pizzaria-roma")
        => new(Guid.NewGuid(), name, slug, null, null, null, true, DateTime.UtcNow);

    [Fact]
    public async Task GetAll_ReturnsAllRestaurants()
    {
        var restaurants = new List<Restaurant> { MakeRestaurant("A"), MakeRestaurant("B") };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(restaurants);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<Restaurant>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyCollection()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<Restaurant>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreated()
    {
        var command = new CreateRestaurantCommand("Sushi Bar", "sushi-bar", "Rua A, 1", "11-1234-5678");
        var dto = MakeRestaurantDto("Sushi Bar", "sushi-bar");
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Create(command);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Create_CallsMediatorSend()
    {
        var command = new CreateRestaurantCommand("New Rest", "new-rest", null, null);
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeRestaurantDto("New Rest", "new-rest"));

        await _controller.Create(command);

        _mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithDto()
    {
        var command = new CreateRestaurantCommand("Churrasco", "churrasco", null, null);
        var dto = MakeRestaurantDto("Churrasco", "churrasco");
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Create(command);

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal(dto, created.Value);
    }
}
