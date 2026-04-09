using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Queries;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants/{restaurantId:guid}/stock")]
public class AdminStockController(IMediator mediator, IStockItemRepository stockRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStockItems(Guid restaurantId)
    {
        var items = await mediator.Send(new GetStockItemsQuery(restaurantId));
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStockItem(Guid restaurantId, [FromBody] CreateStockItemRequest request)
    {
        var item = new StockItem
        {
            RestaurantId = restaurantId,
            Name = request.Name,
            Unit = request.Unit,
            Quantity = request.Quantity,
            MinQuantity = request.MinQuantity
        };

        await stockRepo.AddAsync(item);

        var dto = new StockItemDto(item.Id, item.RestaurantId, item.Name, item.Unit, item.Quantity, item.MinQuantity, item.LastRestockedAt);
        return Created($"/api/admin/restaurants/{restaurantId}/stock/{item.Id}", dto);
    }
}

[ApiController]
[Route("api/admin/stock/{stockItemId:guid}/movements")]
public class AdminStockMovementsController(IMediator mediator, IStockMovementRepository movementRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMovements(Guid stockItemId)
    {
        var movements = await movementRepo.GetByStockItemIdAsync(stockItemId);
        return Ok(movements.Select(m => new StockMovementDto(m.Id, m.StockItemId, m.Type, m.Quantity, m.Reason, m.CreatedAt)));
    }

    [HttpPost]
    public async Task<IActionResult> AddMovement(Guid stockItemId, [FromBody] AddMovementRequest request)
    {
        var movement = await mediator.Send(new AddStockMovementCommand(stockItemId, request.Type, request.Quantity, request.Reason));
        return Created($"/api/admin/stock/{stockItemId}/movements/{movement.Id}", movement);
    }
}

public record CreateStockItemRequest(string Name, StockUnit Unit, decimal Quantity, decimal MinQuantity);
public record AddMovementRequest(StockMovementType Type, decimal Quantity, string? Reason);
