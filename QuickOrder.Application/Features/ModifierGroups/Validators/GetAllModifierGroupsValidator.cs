using FluentValidation;
using QuickOrder.Application.Features.ModifierGroups.Queries;

namespace QuickOrder.Application.Features.ModifierGroups.Validators;

public class GetAllModifierGroupsValidator : AbstractValidator<GetAllModifierGroupsQuery>
{
    public GetAllModifierGroupsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
