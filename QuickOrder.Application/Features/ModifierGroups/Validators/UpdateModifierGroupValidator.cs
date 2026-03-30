using FluentValidation;
using QuickOrder.Application.Features.ModifierGroups.Commands;

namespace QuickOrder.Application.Features.ModifierGroups.Validators;

public class UpdateModifierGroupValidator : AbstractValidator<UpdateModifierGroupCommand>
{
    public UpdateModifierGroupValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del grupo es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x.MinSelections)
            .GreaterThanOrEqualTo(0).WithMessage("La selección mínima no puede ser negativa.")
            .When(x => x.MinSelections.HasValue);

        RuleFor(x => x.MaxSelections)
            .GreaterThan(0).WithMessage("La selección máxima debe ser mayor a cero.")
            .When(x => x.MaxSelections.HasValue);
    }
}
