using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record CreateRestaurantCommand(string Name, string Slug, string? Address, string? Phone) : IRequest<RestaurantDto>;

public sealed class CreateRestaurantHandler(IRestaurantRepository repo) : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
{
    public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken ct = default)
    {
        var restaurant = new Restaurant
        {
            Name = request.Name,
            Slug = request.Slug,
            Address = request.Address,
            Phone = request.Phone
        };

        await repo.AddAsync(restaurant, ct);

        return new RestaurantDto(
            restaurant.Id, restaurant.Name, restaurant.Slug,
            restaurant.Address, restaurant.Phone, restaurant.LogoUrl,
            restaurant.Active, restaurant.CreatedAt);
    }
}
