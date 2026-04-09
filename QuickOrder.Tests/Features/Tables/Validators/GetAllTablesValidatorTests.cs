using QuickOrder.Application.Features.Tables.Queries;
using QuickOrder.Application.Features.Tables.Validators;

namespace QuickOrder.Tests.Features.Tables.Validators;

public class GetAllTablesValidatorTests
{
    private readonly GetAllTablesValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_Passes()
    {
        var result = _validator.Validate(new GetAllTablesQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetAllTablesQuery(0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetAllTablesQuery(1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetAllTablesQuery(1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }
}
