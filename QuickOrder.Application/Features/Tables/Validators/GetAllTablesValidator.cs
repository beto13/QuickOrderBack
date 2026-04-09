using FluentValidation;
using QuickOrder.Application.Features.Tables.Queries;

namespace QuickOrder.Application.Features.Tables.Validators;

public class GetAllTablesValidator : AbstractValidator<GetAllTablesQuery>
{
    public GetAllTablesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize debe estar entre 1 y 100.");
    }
}
