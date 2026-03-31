using FluentValidation;
using QuickOrder.Application.Features.Categories.Commands;

namespace QuickOrder.Application.Features.Categories.Validators;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de la categoría debe ser mayor a 0.");
    }
}
