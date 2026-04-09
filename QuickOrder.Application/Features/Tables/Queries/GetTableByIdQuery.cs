using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Queries;

public record GetTableByIdQuery(int Id) : IRequest<Result<TableDto>>;

public class GetTableByIdQueryHandler(ITableRepository tableRepository) : IRequestHandler<GetTableByIdQuery, Result<TableDto>>
{
    public async Task<Result<TableDto>> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken);
        if (table is null)
            return Result<TableDto>.Fail(Error.NotFound($"Mesa {request.Id} no encontrada."));

        return Result<TableDto>.Ok(new TableDto(table.Id, table.Number, table.IsActive, table.MenuId));
    }
}
