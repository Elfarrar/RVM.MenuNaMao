using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/tables")]
public class TablesController(IMediator mediator) : ControllerBase
{
    [HttpGet("resolve")]
    public async Task<IActionResult> ResolveTable([FromQuery] string token)
    {
        var table = await mediator.Send(new ResolveTableQuery(token));
        return Ok(table);
    }
}
