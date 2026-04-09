namespace RVM.MenuNaMao.Application.DTOs;

public record RestaurantDto(
    Guid Id,
    string Name,
    string Slug,
    string? Address,
    string? Phone,
    string? LogoUrl,
    bool Active,
    DateTime CreatedAt);
