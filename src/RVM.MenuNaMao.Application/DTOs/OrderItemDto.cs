using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.DTOs;

public record OrderItemDto(
    Guid Id,
    Guid MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    string? Notes,
    OrderItemStatus Status);
