using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Commands;

public record DeleteTableCommand(int Id) : IRequest;

public class DeleteTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTableCommand>
{
    public async Task Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Mesa {request.Id} no encontrada.");

        table.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
