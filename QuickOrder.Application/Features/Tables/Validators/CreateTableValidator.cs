using FluentValidation;
using QuickOrder.Application.Features.Tables.Commands;

namespace QuickOrder.Application.Features.Tables.Validators;

public class CreateTableValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("El número de mesa es requerido.")
            .MaximumLength(20).WithMessage("El número de mesa no puede superar los 20 caracteres.");
    }
}
