using FluentValidation;
using QuickOrder.Application.Features.Products.Queries;

namespace QuickOrder.Application.Features.Products.Validators;

public class GetAllProductsValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
