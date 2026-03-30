using FluentValidation;
using QuickOrder.Application.Features.Tables.Commands;

namespace QuickOrder.Application.Features.Tables.Validators;

public class UpdateTableValidator : AbstractValidator<UpdateTableCommand>
{
    public UpdateTableValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id de la mesa es requerido.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("El número de mesa no puede ser vacío.")
            .MaximumLength(20).WithMessage("El número de mesa no puede superar los 20 caracteres.")
            .When(x => x.Number is not null);
    }
}
