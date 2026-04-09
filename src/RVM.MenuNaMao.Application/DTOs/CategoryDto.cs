namespace RVM.MenuNaMao.Application.DTOs;

public record CategoryDto(
    Guid Id,
    Guid RestaurantId,
    string Name,
    int DisplayOrder,
    bool Active);
