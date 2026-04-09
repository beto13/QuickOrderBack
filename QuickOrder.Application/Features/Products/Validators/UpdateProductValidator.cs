using FluentValidation;
using QuickOrder.Application.Features.Products.Commands;

namespace QuickOrder.Application.Features.Products.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id del producto es requerido.");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.")
            .When(x => x.Description is not null);
    }
}
