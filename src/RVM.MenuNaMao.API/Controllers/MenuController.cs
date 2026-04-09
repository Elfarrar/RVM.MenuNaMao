using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController(IMediator mediator) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetMenu(string slug)
    {
        var menu = await mediator.Send(new GetMenuQuery(slug));
        return Ok(menu);
    }
}
