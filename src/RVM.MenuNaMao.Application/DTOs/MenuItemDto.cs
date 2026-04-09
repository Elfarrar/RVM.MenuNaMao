namespace RVM.MenuNaMao.Application.DTOs;

public record MenuItemDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    bool Available,
    int PreparationTimeMinutes);
