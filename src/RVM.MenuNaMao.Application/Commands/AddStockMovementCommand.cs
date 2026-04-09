using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Enums;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record AddStockMovementCommand(Guid StockItemId, StockMovementType Type, decimal Quantity, string? Reason) : IRequest<StockMovementDto>;

public sealed class AddStockMovementHandler(
    IStockItemRepository stockRepo,
    IStockMovementRepository movementRepo,
    IMediator mediator)
    : IRequestHandler<AddStockMovementCommand, StockMovementDto>
{
    public async Task<StockMovementDto> Handle(AddStockMovementCommand request, CancellationToken ct = default)
    {
        var stockItem = await stockRepo.GetByIdAsync(request.StockItemId, ct)
            ?? throw new KeyNotFoundException($"StockItem {request.StockItemId} not found");

        var movement = new StockMovement
        {
            StockItemId = stockItem.Id,
            Type = request.Type,
            Quantity = request.Quantity,
            Reason = request.Reason
        };

        switch (request.Type)
        {
            case StockMovementType.In:
                stockItem.Quantity += request.Quantity;
                stockItem.LastRestockedAt = DateTime.UtcNow;
                break;
            case StockMovementType.Out:
                stockItem.Quantity -= request.Quantity;
                break;
            case StockMovementType.Adjustment:
                stockItem.Quantity = request.Quantity;
                break;
        }

        await movementRepo.AddAsync(movement, ct);
        await stockRepo.UpdateAsync(stockItem, ct);

        if (stockItem.Quantity <= stockItem.MinQuantity)
        {
            await mediator.Publish(
                new StockLowNotification(stockItem.Id, stockItem.Name, stockItem.Quantity, stockItem.MinQuantity), ct);
        }

        return new StockMovementDto(movement.Id, movement.StockItemId, movement.Type, movement.Quantity, movement.Reason, movement.CreatedAt);
    }
}
