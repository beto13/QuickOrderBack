using FluentValidation;
using QuickOrder.Application.Features.Modifiers.Queries;

namespace QuickOrder.Application.Features.Modifiers.Validators;

public class GetAllModifiersValidator : AbstractValidator<GetAllModifiersQuery>
{
    public GetAllModifiersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
