using QuickOrder.Application.Features.Menus.Queries;
using QuickOrder.Application.Features.Menus.Validators;

namespace QuickOrder.Tests.Features.Menus.Validators;

public class GetAllMenusValidatorTests
{
    private readonly GetAllMenusValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_Passes()
    {
        var result = _validator.Validate(new GetAllMenusQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetAllMenusQuery(0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetAllMenusQuery(1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetAllMenusQuery(1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }
}
