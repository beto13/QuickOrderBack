using FluentValidation;
using QuickOrder.Application.Features.Menus.Commands;

namespace QuickOrder.Application.Features.Menus.Validators;

public class CreateMenuValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del menú es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");
    }
}
