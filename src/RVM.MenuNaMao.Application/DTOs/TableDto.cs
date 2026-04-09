using RVM.MenuNaMao.Domain.Enums;

namespace RVM.MenuNaMao.Application.DTOs;

public record TableDto(
    Guid Id,
    Guid RestaurantId,
    int Number,
    string QrCodeToken,
    TableStatus Status);
