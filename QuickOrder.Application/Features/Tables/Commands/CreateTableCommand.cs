using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Tables.Commands;

public record CreateTableCommand(string Number) : IRequest<TableDto>;

public class CreateTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTableCommand, TableDto>
{
    public async Task<TableDto> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var table = new Table { Number = request.Number };

        tableRepository.Add(table);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TableDto(table.Id, table.Number, table.IsActive);
    }
}
