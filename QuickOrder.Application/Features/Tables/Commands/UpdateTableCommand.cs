using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Commands;

public record UpdateTableCommand(int Id, string? Number, bool? IsActive, int? MenuId) : IRequest<TableDto>;

public class UpdateTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTableCommand, TableDto>
{
    public async Task<TableDto> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Mesa {request.Id} no encontrada.");

        if (request.Number is not null) table.Number = request.Number;
        if (request.IsActive.HasValue) table.IsActive = request.IsActive.Value;
        if (request.MenuId.HasValue) table.MenuId = request.MenuId.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TableDto(table.Id, table.Number, table.IsActive, table.MenuId);
    }
}
