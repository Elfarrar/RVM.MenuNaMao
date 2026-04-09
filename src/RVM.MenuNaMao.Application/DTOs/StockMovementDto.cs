using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.DTOs;

public record StockMovementDto(
    Guid Id,
    Guid StockItemId,
    StockMovementType Type,
    decimal Quantity,
    string? Reason,
    DateTime CreatedAt);
