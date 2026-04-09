using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Commands;

public record DeleteTableCommand(int Id) : IRequest<Result>;

public class DeleteTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTableCommand, Result>
{
    public async Task<Result> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.Id, cancellationToken);
        if (table is null)
            return Result.Fail(Error.NotFound($"Mesa {request.Id} no encontrada."));

        table.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
