using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, int DisplayOrder) : IRequest<Result<CategoryDto>>;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            DisplayOrder = request.DisplayOrder
        };

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Ok(new CategoryDto(category.Id, category.Name, category.DisplayOrder));
    }
}
