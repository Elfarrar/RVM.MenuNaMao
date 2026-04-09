namespace RVM.MenuNaMao.Application.DTOs;

public record MenuDto(
    RestaurantDto Restaurant,
    List<CategoryWithItemsDto> Categories);

public record CategoryWithItemsDto(
    Guid Id,
    string Name,
    int DisplayOrder,
    List<MenuItemDto> Items);
