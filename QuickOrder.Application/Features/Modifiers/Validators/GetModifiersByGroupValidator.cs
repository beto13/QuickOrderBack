using FluentValidation;
using QuickOrder.Application.Features.Modifiers.Queries;

namespace QuickOrder.Application.Features.Modifiers.Validators;

public class GetModifiersByGroupValidator : AbstractValidator<GetModifiersByGroupQuery>
{
    public GetModifiersByGroupValidator()
    {
        RuleFor(x => x.ModifierGroupId)
            .GreaterThan(0).WithMessage("ModifierGroupId es requerido.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
