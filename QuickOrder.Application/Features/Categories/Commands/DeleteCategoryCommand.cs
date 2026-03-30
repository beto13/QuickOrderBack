using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoría {request.Id} no encontrada.");

        categoryRepository.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
