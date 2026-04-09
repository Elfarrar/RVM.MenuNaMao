using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
public class AdminOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] Guid restaurantId, [FromQuery] OrderStatus? status = null)
    {
        var orders = await mediator.Send(new GetRestaurantOrdersQuery(restaurantId, status));
        return Ok(orders);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        await mediator.Send(new UpdateOrderStatusCommand(id, request.Status));
        return NoContent();
    }
}

public record UpdateStatusRequest(OrderStatus Status);
