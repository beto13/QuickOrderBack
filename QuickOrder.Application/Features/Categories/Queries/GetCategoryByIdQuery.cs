using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Queries;

public record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryDto>>;

public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Fail(Error.NotFound($"Categoría {request.Id} no encontrada."));

        return Result<CategoryDto>.Ok(new CategoryDto(category.Id, category.Name, category.DisplayOrder));
    }
}
