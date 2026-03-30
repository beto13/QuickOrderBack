using FluentValidation;
using QuickOrder.Application.Features.Menus.Commands;

namespace QuickOrder.Application.Features.Menus.Validators;

public class UpdateMenuValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del menú es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .When(x => x.Name is not null);
    }
}
