using QuickOrder.Application.Features.Categories.Queries;
using QuickOrder.Application.Features.Categories.Validators;

namespace QuickOrder.Tests.Features.Categories.Validators;

public class GetAllCategoriesValidatorTests
{
    private readonly GetAllCategoriesValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_Passes()
    {
        var result = _validator.Validate(new GetAllCategoriesQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetAllCategoriesQuery(0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetAllCategoriesQuery(1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetAllCategoriesQuery(1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }
}
