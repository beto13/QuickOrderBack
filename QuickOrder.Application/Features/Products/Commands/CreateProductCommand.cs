using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Products.Commands;

public record CreateProductCommand(string Name, string? Description) : IRequest<Result<ProductDto>>;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description
        };

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Ok(new ProductDto(product.Id, product.Name, product.Description, product.ImageUrl));
    }
}
