using FluentValidation;
using QuickOrder.Application.Features.Modifiers.Commands;

namespace QuickOrder.Application.Features.Modifiers.Validators;

public class DeleteModifierValidator : AbstractValidator<DeleteModifierCommand>
{
    public DeleteModifierValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del modificador debe ser mayor a 0.");
    }
}
