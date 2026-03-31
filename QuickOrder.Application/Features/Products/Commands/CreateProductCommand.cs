using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Products.Commands;

public record CreateProductCommand(string Name, string? Description) : IRequest<ProductDto>;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description
        };

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto(product.Id, product.Name, product.Description, product.ImageUrl);
    }
}
