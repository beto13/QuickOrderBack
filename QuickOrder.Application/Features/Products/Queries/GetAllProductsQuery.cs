using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Queries;

public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedResponse<ProductDto>>;

public class GetAllProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetAllProductsQuery, PaginatedResponse<ProductDto>>
{
    public async Task<PaginatedResponse<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await productRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.ImageUrl)).ToList();

        return PaginatedResponse<ProductDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
