using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record CreateCategoryCommand(Guid RestaurantId, string Name, int DisplayOrder) : IRequest<CategoryDto>;

public sealed class CreateCategoryHandler(ICategoryRepository repo) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct = default)
    {
        var category = new Category
        {
            RestaurantId = request.RestaurantId,
            Name = request.Name,
            DisplayOrder = request.DisplayOrder
        };

        await repo.AddAsync(category, ct);

        return new CategoryDto(category.Id, category.RestaurantId, category.Name, category.DisplayOrder, category.Active);
    }
}
