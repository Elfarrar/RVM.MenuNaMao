using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetMenuQuery(string RestaurantSlug) : IRequest<MenuDto>;

public sealed class GetMenuHandler(
    IRestaurantRepository restaurantRepo,
    ICategoryRepository categoryRepo,
    IMenuItemRepository menuItemRepo) : IRequestHandler<GetMenuQuery, MenuDto>
{
    public async Task<MenuDto> Handle(GetMenuQuery request, CancellationToken ct = default)
    {
        var restaurant = await restaurantRepo.GetBySlugAsync(request.RestaurantSlug, ct)
            ?? throw new KeyNotFoundException($"Restaurant '{request.RestaurantSlug}' not found");

        var categories = await categoryRepo.GetByRestaurantIdAsync(restaurant.Id, activeOnly: true, ct);
        var categoryDtos = new List<CategoryWithItemsDto>();

        foreach (var cat in categories.OrderBy(c => c.DisplayOrder))
        {
            var items = await menuItemRepo.GetByCategoryIdAsync(cat.Id, availableOnly: true, ct);
            var itemDtos = items.Select(i => new MenuItemDto(
                i.Id, i.CategoryId, i.Name, i.Description,
                i.Price, i.ImageUrl, i.Available, i.PreparationTimeMinutes)).ToList();

            categoryDtos.Add(new CategoryWithItemsDto(cat.Id, cat.Name, cat.DisplayOrder, itemDtos));
        }

        var restaurantDto = new RestaurantDto(
            restaurant.Id, restaurant.Name, restaurant.Slug,
            restaurant.Address, restaurant.Phone, restaurant.LogoUrl,
            restaurant.Active, restaurant.CreatedAt);

        return new MenuDto(restaurantDto, categoryDtos);
    }
}
