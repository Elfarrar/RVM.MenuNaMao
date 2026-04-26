using Microsoft.AspNetCore.Mvc;
using Moq;
using RVM.MenuNaMao.API.Controllers;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;

namespace RVM.MenuNaMao.Tests.Controllers;

public class AdminDashboardControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly AdminDashboardController _controller;

    public AdminDashboardControllerTests()
    {
        _controller = new AdminDashboardController(_mediator.Object);
    }

    private static DashboardDto MakeDashboard(int pending = 3, int preparing = 2, int today = 10, decimal revenue = 500m)
        => new(pending, preparing, today, revenue, []);

    [Fact]
    public async Task GetDashboard_ReturnsOk()
    {
        var restaurantId = Guid.NewGuid();
        var dashboard = MakeDashboard();
        _mediator.Setup(m => m.Send(It.IsAny<GetDashboardQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dashboard);

        var result = await _controller.GetDashboard(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<DashboardDto>(ok.Value);
    }

    [Fact]
    public async Task GetDashboard_PassesRestaurantIdToQuery()
    {
        var restaurantId = Guid.NewGuid();
        _mediator.Setup(m => m.Send(It.IsAny<GetDashboardQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(MakeDashboard());

        await _controller.GetDashboard(restaurantId);

        _mediator.Verify(m => m.Send(
            It.Is<GetDashboardQuery>(q => q.RestaurantId == restaurantId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDashboard_ReturnsDashboardWithStockAlerts()
    {
        var restaurantId = Guid.NewGuid();
        var alerts = new List<StockAlertDto>
        {
            new(Guid.NewGuid(), "Queijo", 0.5m, 1m),
            new(Guid.NewGuid(), "Farinha", 2m, 5m)
        };
        var dashboard = new DashboardDto(1, 0, 5, 200m, alerts);
        _mediator.Setup(m => m.Send(It.IsAny<GetDashboardQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(dashboard);

        var result = await _controller.GetDashboard(restaurantId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<DashboardDto>(ok.Value);
        Assert.Equal(2, dto.StockAlerts.Count);
        Assert.Equal(1, dto.PendingOrders);
        Assert.Equal(200m, dto.TodayRevenue);
    }
}
