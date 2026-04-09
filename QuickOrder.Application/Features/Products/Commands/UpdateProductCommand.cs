using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Commands;

public record UpdateProductCommand(int Id, string? Name, string? Description) : IRequest<Result<ProductDto>>;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result<ProductDto>.Fail(Error.NotFound($"Producto {request.Id} no encontrado."));

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Ok(new ProductDto(product.Id, product.Name, product.Description, product.ImageUrl));
    }
}
