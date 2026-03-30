using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Queries;

public record GetAllCategoriesQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedResponse<CategoryDto>>;

public class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetAllCategoriesQuery, PaginatedResponse<CategoryDto>>
{
    public async Task<PaginatedResponse<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await categoryRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(c => new CategoryDto(c.Id, c.Name, c.DisplayOrder)).ToList();

        return PaginatedResponse<CategoryDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
