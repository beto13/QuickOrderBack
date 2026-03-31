using FluentValidation;
using QuickOrder.Application.Features.Tables.Commands;

namespace QuickOrder.Application.Features.Tables.Validators;

public class DeleteTableValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de la mesa debe ser mayor a 0.");
    }
}
