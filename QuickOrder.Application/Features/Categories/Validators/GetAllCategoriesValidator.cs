using FluentValidation;
using QuickOrder.Application.Features.Categories.Queries;

namespace QuickOrder.Application.Features.Categories.Validators;

public class GetAllCategoriesValidator : AbstractValidator<GetAllCategoriesQuery>
{
    public GetAllCategoriesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
