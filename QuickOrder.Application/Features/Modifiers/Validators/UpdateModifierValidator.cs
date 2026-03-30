using FluentValidation;
using QuickOrder.Application.Features.Modifiers.Commands;

namespace QuickOrder.Application.Features.Modifiers.Validators;

public class UpdateModifierValidator : AbstractValidator<UpdateModifierCommand>
{
    public UpdateModifierValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del modificador es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.")
            .When(x => x.Description is not null);
    }
}
