using FluentValidation;
using QuickOrder.Application.Features.Menus.Commands;

namespace QuickOrder.Application.Features.Menus.Validators;

public class DeleteMenuValidator : AbstractValidator<DeleteMenuCommand>
{
    public DeleteMenuValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del menú debe ser mayor a 0.");
    }
}
