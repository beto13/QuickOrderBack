using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Queries;

public record GetTableByNumberQuery(string Number) : IRequest<Result<TableDto>>;

public class GetTableByNumberQueryHandler(ITableRepository tableRepository) : IRequestHandler<GetTableByNumberQuery, Result<TableDto>>
{
    public async Task<Result<TableDto>> Handle(GetTableByNumberQuery request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByNumberAsync(request.Number, cancellationToken);
        if (table is null)
            return Result<TableDto>.Fail(Error.NotFound($"Mesa {request.Number} no encontrada."));

        return Result<TableDto>.Ok(new TableDto(table.Id, table.Number, table.IsActive, table.MenuId));
    }
}
