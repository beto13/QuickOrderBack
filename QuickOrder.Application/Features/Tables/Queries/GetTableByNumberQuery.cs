using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Queries;

public record GetTableByNumberQuery(string Number) : IRequest<TableDto>;

public class GetTableByNumberQueryHandler(ITableRepository tableRepository) : IRequestHandler<GetTableByNumberQuery, TableDto>
{
    public async Task<TableDto> Handle(GetTableByNumberQuery request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByNumberAsync(request.Number, cancellationToken)
            ?? throw new KeyNotFoundException($"Mesa {request.Number} no encontrada.");

        return new TableDto(table.Id, table.Number, table.IsActive, table.MenuId);
    }
}
