using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<Result>;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Fail(Error.NotFound($"Producto {request.Id} no encontrado."));

        productRepository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
