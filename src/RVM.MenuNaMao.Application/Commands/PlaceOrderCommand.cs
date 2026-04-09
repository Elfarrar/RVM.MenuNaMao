using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Application.Notifications;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record PlaceOrderItemRequest(Guid MenuItemId, int Quantity, string? Notes);

public record PlaceOrderCommand(Guid TableId, string? CustomerName, List<PlaceOrderItemRequest> Items) : IRequest<OrderDto>;

public sealed class PlaceOrderHandler(
    IOrderRepository orderRepo,
    ITableRepository tableRepo,
    IMenuItemRepository menuItemRepo,
    IMediator mediator) : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken ct = default)
    {
        var table = await tableRepo.GetByIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Table {request.TableId} not found");

        var order = new Order
        {
            RestaurantId = table.RestaurantId,
            TableId = table.Id,
            CustomerName = request.CustomerName
        };

        var itemDtos = new List<OrderItemDto>();

        foreach (var reqItem in request.Items)
        {
            var menuItem = await menuItemRepo.GetByIdAsync(reqItem.MenuItemId, ct)
                ?? throw new KeyNotFoundException($"MenuItem {reqItem.MenuItemId} not found");

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                MenuItemId = menuItem.Id,
                Quantity = reqItem.Quantity,
                UnitPrice = menuItem.Price,
                Notes = reqItem.Notes
            };

            order.Items.Add(orderItem);
            itemDtos.Add(new OrderItemDto(
                orderItem.Id, orderItem.MenuItemId, menuItem.Name,
                orderItem.Quantity, orderItem.UnitPrice, orderItem.Notes, orderItem.Status));
        }

        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        await orderRepo.AddAsync(order, ct);

        await mediator.Publish(new OrderPlacedNotification(order.Id, order.RestaurantId, order.TableId), ct);

        return new OrderDto(
            order.Id, order.RestaurantId, order.TableId, table.Number,
            order.CustomerName, order.Status, order.CreatedAt, order.TotalAmount, itemDtos);
    }
}
