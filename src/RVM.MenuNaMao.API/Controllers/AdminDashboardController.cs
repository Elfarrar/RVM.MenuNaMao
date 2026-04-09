using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants/{restaurantId:guid}/dashboard")]
public class AdminDashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboard(Guid restaurantId)
    {
        var dashboard = await mediator.Send(new GetDashboardQuery(restaurantId));
        return Ok(dashboard);
    }
}
