using RVM.MenuNaMao.Application.Commands;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RVM.MenuNaMao.API.Controllers;

[ApiController]
[Route("api/admin/restaurants")]
public class AdminRestaurantsController(IMediator mediator, IRestaurantRepository restaurantRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restaurants = await restaurantRepo.GetAllAsync();
        return Ok(restaurants);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRestaurantCommand command)
    {
        var restaurant = await mediator.Send(command);
        return Created($"/api/admin/restaurants/{restaurant.Id}", restaurant);
    }
}
