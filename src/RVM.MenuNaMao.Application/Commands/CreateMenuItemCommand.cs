using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record CreateMenuItemCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int PreparationTimeMinutes) : IRequest<MenuItemDto>;

public sealed class CreateMenuItemHandler(IMenuItemRepository repo) : IRequestHandler<CreateMenuItemCommand, MenuItemDto>
{
    public async Task<MenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken ct = default)
    {
        var item = new MenuItem
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            PreparationTimeMinutes = request.PreparationTimeMinutes
        };

        await repo.AddAsync(item, ct);

        return new MenuItemDto(
            item.Id, item.CategoryId, item.Name, item.Description,
            item.Price, item.ImageUrl, item.Available, item.PreparationTimeMinutes);
    }
}
