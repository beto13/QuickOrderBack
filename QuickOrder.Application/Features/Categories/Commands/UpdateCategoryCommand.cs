using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(int Id, string? Name, int? DisplayOrder) : IRequest<Result<CategoryDto>>;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Fail(Error.NotFound($"Categoría {request.Id} no encontrada."));

        if (request.Name is not null) category.Name = request.Name;
        if (request.DisplayOrder.HasValue) category.DisplayOrder = request.DisplayOrder.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Ok(new CategoryDto(category.Id, category.Name, category.DisplayOrder));
    }
}
