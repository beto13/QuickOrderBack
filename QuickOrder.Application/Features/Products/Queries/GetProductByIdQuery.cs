using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result<ProductDto>.Fail(Error.NotFound($"Producto {request.Id} no encontrado."));

        return Result<ProductDto>.Ok(new ProductDto(product.Id, product.Name, product.Description, product.ImageUrl));
    }
}
