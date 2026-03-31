using FluentValidation;
using QuickOrder.Application.Features.Products.Commands;

namespace QuickOrder.Application.Features.Products.Validators;

public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del producto debe ser mayor a 0.");
    }
}
