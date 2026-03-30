using FluentValidation;
using QuickOrder.Application.Features.Categories.Commands;

namespace QuickOrder.Application.Features.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id de la categoría es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualización no puede ser negativo.")
            .When(x => x.DisplayOrder.HasValue);
    }
}
