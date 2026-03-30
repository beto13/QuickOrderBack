using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(int Id, string? Name, int? DisplayOrder) : IRequest<CategoryDto>;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoría {request.Id} no encontrada.");

        if (request.Name is not null) category.Name = request.Name;
        if (request.DisplayOrder.HasValue) category.DisplayOrder = request.DisplayOrder.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.DisplayOrder);
    }
}
