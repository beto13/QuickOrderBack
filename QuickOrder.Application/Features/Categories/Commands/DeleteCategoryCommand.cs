using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest<Result>;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Fail(Error.NotFound($"Categoría {request.Id} no encontrada."));

        categoryRepository.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
