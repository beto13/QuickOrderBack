using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Queries;

public record GetTableByIdQuery(int Id) : IRequest<TableDto>;

public class GetTableByIdQueryHandler(ITableRepository tableRepository) : IRequestHandler<GetTableByIdQuery, TableDto>
{
    public async Task<TableDto> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Mesa {request.Id} no encontrada.");

        return new TableDto(table.Id, table.Number, table.IsActive, table.MenuId);
    }
}
