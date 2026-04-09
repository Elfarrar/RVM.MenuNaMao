using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.DTOs;

public record StockItemDto(
    Guid Id,
    Guid RestaurantId,
    string Name,
    StockUnit Unit,
    decimal Quantity,
    decimal MinQuantity,
    DateTime? LastRestockedAt);
