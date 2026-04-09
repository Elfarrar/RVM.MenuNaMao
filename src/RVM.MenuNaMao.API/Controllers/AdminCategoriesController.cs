using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants/{restaurantId:guid}/categories")]
public class AdminCategoriesController(IMediator mediator, ICategoryRepository categoryRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(Guid restaurantId)
    {
        var categories = await categoryRepo.GetByRestaurantIdAsync(restaurantId);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid restaurantId, [FromBody] CreateCategoryRequest request)
    {
        var category = await mediator.Send(new CreateCategoryCommand(restaurantId, request.Name, request.DisplayOrder));
        return Created($"/api/admin/restaurants/{restaurantId}/categories/{category.Id}", category);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid restaurantId, Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await categoryRepo.GetByIdAsync(id);
        if (category is null) return NotFound();

        category.Name = request.Name;
        category.DisplayOrder = request.DisplayOrder;
        category.Active = request.Active;
        await categoryRepo.UpdateAsync(category);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid restaurantId, Guid id)
    {
        await categoryRepo.DeleteAsync(id);
        return NoContent();
    }
}

public record CreateCategoryRequest(string Name, int DisplayOrder);
public record UpdateCategoryRequest(string Name, int DisplayOrder, bool Active);
