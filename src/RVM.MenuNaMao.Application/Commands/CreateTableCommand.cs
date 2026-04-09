using RVM.MenuNaMao.Application.DTOs;
using RVM.MenuNaMao.Application.Mediator;
using RVM.MenuNaMao.Domain.Entities;
using RVM.MenuNaMao.Domain.Interfaces;

namespace RVM.MenuNaMao.Application.Commands;

public record CreateTableCommand(Guid RestaurantId, int Number) : IRequest<TableDto>;

public sealed class CreateTableHandler(ITableRepository repo) : IRequestHandler<CreateTableCommand, TableDto>
{
    public async Task<TableDto> Handle(CreateTableCommand request, CancellationToken ct = default)
    {
        var table = new Table
        {
            RestaurantId = request.RestaurantId,
            Number = request.Number,
            QrCodeToken = Guid.CreateVersion7().ToString("N")
        };

        await repo.AddAsync(table, ct);

        return new TableDto(table.Id, table.RestaurantId, table.Number, table.QrCodeToken, table.Status);
    }
}
