using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Commands;

public record UpdateProductCommand(int Id, string? Name, string? Description) : IRequest<ProductDto>;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Producto {request.Id} no encontrado.");

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(product.Id, product.Name, product.Description);
    }
}
