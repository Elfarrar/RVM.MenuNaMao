using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Queries;

public record ResolveTableQuery(string QrCodeToken) : IRequest<TableDto>;

public sealed class ResolveTableHandler(ITableRepository repo)
    : IRequestHandler<ResolveTableQuery, TableDto>
{
    public async Task<TableDto> Handle(ResolveTableQuery request, CancellationToken ct = default)
    {
        var table = await repo.GetByQrCodeTokenAsync(request.QrCodeToken, ct)
            ?? throw new KeyNotFoundException($"Table with token '{request.QrCodeToken}' not found");

        return new TableDto(table.Id, table.RestaurantId, table.Number, table.QrCodeToken, table.Status);
    }
}
