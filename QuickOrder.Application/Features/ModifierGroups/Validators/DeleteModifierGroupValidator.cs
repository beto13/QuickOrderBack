using FluentValidation;
using QuickOrder.Application.Features.ModifierGroups.Commands;

namespace QuickOrder.Application.Features.ModifierGroups.Validators;

public class DeleteModifierGroupValidator : AbstractValidator<DeleteModifierGroupCommand>
{
    public DeleteModifierGroupValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del grupo de modificadores debe ser mayor a 0.");
    }
}
