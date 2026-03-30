using FluentValidation;
using QuickOrder.Application.Features.Orders.Commands;

namespace QuickOrder.Application.Features.Orders.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0).WithMessage("La mesa es requerida.");

        RuleFor(x => x.MenuId)
            .GreaterThan(0).WithMessage("El menú es requerido.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("El pedido debe tener al menos un producto.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.MenuProductId)
                .GreaterThan(0).WithMessage("El producto es requerido.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        });

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Las notas no pueden superar los 500 caracteres.")
            .When(x => x.Notes is not null);
    }
}
