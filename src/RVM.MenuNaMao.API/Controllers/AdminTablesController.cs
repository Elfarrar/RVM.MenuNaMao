using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants/{restaurantId:guid}/tables")]
public class AdminTablesController(IMediator mediator, IQrCodeService qrCodeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTables(Guid restaurantId)
    {
        var tables = await mediator.Send(new GetTablesQuery(restaurantId));
        return Ok(tables);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTable(Guid restaurantId, [FromBody] CreateTableRequest request)
    {
        var table = await mediator.Send(new CreateTableCommand(restaurantId, request.Number));
        return Created($"/api/admin/restaurants/{restaurantId}/tables/{table.Id}", table);
    }

    [HttpGet("{tableId:guid}/qrcode")]
    public async Task<IActionResult> GetQrCode(Guid restaurantId, Guid tableId)
    {
        var tables = await mediator.Send(new GetTablesQuery(restaurantId));
        var table = tables.FirstOrDefault(t => t.Id == tableId);
        if (table is null) return NotFound();

        var url = $"{Request.Scheme}://{Request.Host}/menu?table={table.QrCodeToken}";
        var png = qrCodeService.GenerateQrCodePng(url);
        return File(png, "image/png", $"table-{table.Number}-qrcode.png");
    }
}

public record CreateTableRequest(int Number);
