using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Commands;

public record UpdateTableCommand(int Id, string? Number, bool? IsActive, int? MenuId) : IRequest<Result<TableDto>>;

public class UpdateTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTableCommand, Result<TableDto>>
{
    public async Task<Result<TableDto>> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken);
        if (table is null)
            return Result<TableDto>.Fail(Error.NotFound($"Mesa {request.Id} no encontrada."));

        if (request.Number is not null) table.Number = request.Number;
        if (request.IsActive.HasValue) table.IsActive = request.IsActive.Value;
        if (request.MenuId.HasValue) table.MenuId = request.MenuId.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TableDto>.Ok(new TableDto(table.Id, table.Number, table.IsActive, table.MenuId));
    }
}
