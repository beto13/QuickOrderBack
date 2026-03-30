using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Producto {request.Id} no encontrado.");

        productRepository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
