using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants/{restaurantId:guid}/menu-items")]
public class AdminMenuItemsController(IMediator mediator, IMenuItemRepository menuItemRepo, ICategoryRepository categoryRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMenuItems(Guid restaurantId)
    {
        var categories = await categoryRepo.GetByRestaurantIdAsync(restaurantId);
        var items = new List<object>();
        foreach (var cat in categories)
        {
            var catItems = await menuItemRepo.GetByCategoryIdAsync(cat.Id);
            items.AddRange(catItems);
        }
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid restaurantId, [FromBody] CreateMenuItemCommand command)
    {
        var item = await mediator.Send(command);
        return Created($"/api/admin/restaurants/{restaurantId}/menu-items/{item.Id}", item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid restaurantId, Guid id, [FromBody] UpdateMenuItemRequest request)
    {
        var item = await menuItemRepo.GetByIdAsync(id);
        if (item is null) return NotFound();

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.ImageUrl = request.ImageUrl;
        item.Available = request.Available;
        item.PreparationTimeMinutes = request.PreparationTimeMinutes;
        await menuItemRepo.UpdateAsync(item);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid restaurantId, Guid id)
    {
        await menuItemRepo.DeleteAsync(id);
        return NoContent();
    }
}

public record UpdateMenuItemRequest(string Name, string? Description, decimal Price, string? ImageUrl, bool Available, int PreparationTimeMinutes);
