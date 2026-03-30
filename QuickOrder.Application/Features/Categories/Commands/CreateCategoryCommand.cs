using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, int DisplayOrder) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            DisplayOrder = request.DisplayOrder
        };

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.DisplayOrder);
    }
}
