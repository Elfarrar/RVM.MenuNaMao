using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid RestaurantId,
    Guid TableId,
    int TableNumber,
    string? CustomerName,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    List<OrderItemDto> Items);
