using FluentValidation;
using QuickOrder.Application.Features.Modifiers.Commands;

namespace QuickOrder.Application.Features.Modifiers.Validators;

public class CreateModifierValidator : AbstractValidator<CreateModifierCommand>
{
    public CreateModifierValidator()
    {
        RuleFor(x => x.ModifierGroupId)
            .GreaterThan(0).WithMessage("El grupo de modificadores es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del modificador es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.")
            .When(x => x.Description is not null);
    }
}
