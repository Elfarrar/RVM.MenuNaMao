using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand command)
    {
        var order = await mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await mediator.Send(new GetOrderQuery(id));
        return Ok(order);
    }
}
