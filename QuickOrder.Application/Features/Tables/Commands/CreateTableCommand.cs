using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Tables.Commands;

public record CreateTableCommand(string Number, int MenuId) : IRequest<Result<TableDto>>;

public class CreateTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTableCommand, Result<TableDto>>
{
    public async Task<Result<TableDto>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var table = new Table { Number = request.Number, MenuId = request.MenuId };

        tableRepository.Add(table);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TableDto>.Ok(new TableDto(table.Id, table.Number, table.IsActive, table.MenuId));
    }
}
