using QuickOrder.Application.Features.Products.Queries;
using QuickOrder.Application.Features.Products.Validators;

namespace QuickOrder.Tests.Features.Products.Validators;

public class GetAllProductsValidatorTests
{
    private readonly GetAllProductsValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_Passes()
    {
        var result = _validator.Validate(new GetAllProductsQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ValidPagination_Passes()
    {
        var result = _validator.Validate(new GetAllProductsQuery(2, 50));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetAllProductsQuery(0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageNumberNegative_Fails()
    {
        var result = _validator.Validate(new GetAllProductsQuery(-1, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetAllProductsQuery(1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetAllProductsQuery(1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeAt100_Passes()
    {
        var result = _validator.Validate(new GetAllProductsQuery(1, 100));
        Assert.True(result.IsValid);
    }
}
