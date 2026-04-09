using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record GetTablesQuery(Guid RestaurantId) : IRequest<List<TableDto>>;

public sealed class GetTablesHandler(ITableRepository repo)
    : IRequestHandler<GetTablesQuery, List<TableDto>>
{
    public async Task<List<TableDto>> Handle(GetTablesQuery request, CancellationToken ct = default)
    {
        var tables = await repo.GetByRestaurantIdAsync(request.RestaurantId, ct);

        return tables.Select(t => new TableDto(
            t.Id, t.RestaurantId, t.Number, t.QrCodeToken, t.Status)).ToList();
    }
}
