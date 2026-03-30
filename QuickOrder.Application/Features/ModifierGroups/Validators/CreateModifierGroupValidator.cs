using FluentValidation;
using QuickOrder.Application.Features.ModifierGroups.Commands;

namespace QuickOrder.Application.Features.ModifierGroups.Validators;

public class CreateModifierGroupValidator : AbstractValidator<CreateModifierGroupCommand>
{
    public CreateModifierGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del grupo es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.MinSelections)
            .GreaterThanOrEqualTo(0).WithMessage("La selección mínima no puede ser negativa.");

        RuleFor(x => x.MaxSelections)
            .GreaterThan(0).WithMessage("La selección máxima debe ser mayor a cero.")
            .GreaterThanOrEqualTo(x => x.MinSelections).WithMessage("La selección máxima debe ser mayor o igual a la mínima.");

        RuleFor(x => x)
            .Must(x => x.ProductId.HasValue || x.CategoryId.HasValue)
            .WithMessage("El grupo debe estar asociado a un producto o a una categoría.");
    }
}
