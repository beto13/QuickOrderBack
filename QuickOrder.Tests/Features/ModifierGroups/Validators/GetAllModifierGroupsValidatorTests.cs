using QuickOrder.Application.Features.ModifierGroups.Queries;
using QuickOrder.Application.Features.ModifierGroups.Validators;

namespace QuickOrder.Tests.Features.ModifierGroups.Validators;

public class GetAllModifierGroupsValidatorTests
{
    private readonly GetAllModifierGroupsValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_Passes()
    {
        var result = _validator.Validate(new GetAllModifierGroupsQuery());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetAllModifierGroupsQuery(0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetAllModifierGroupsQuery(1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetAllModifierGroupsQuery(1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }
}
