using QuickOrder.Application.Features.Modifiers.Queries;
using QuickOrder.Application.Features.Modifiers.Validators;

namespace QuickOrder.Tests.Features.Modifiers.Validators;

public class GetModifiersByGroupValidatorTests
{
    private readonly GetModifiersByGroupValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_Passes()
    {
        var result = _validator.Validate(new GetModifiersByGroupQuery(1, 1, 20));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ModifierGroupIdZero_Fails()
    {
        var result = _validator.Validate(new GetModifiersByGroupQuery(0, 1, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ModifierGroupId");
    }

    [Fact]
    public void Validate_PageNumberZero_Fails()
    {
        var result = _validator.Validate(new GetModifiersByGroupQuery(1, 0, 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validate_PageSizeOver100_Fails()
    {
        var result = _validator.Validate(new GetModifiersByGroupQuery(1, 1, 101));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeZero_Fails()
    {
        var result = _validator.Validate(new GetModifiersByGroupQuery(1, 1, 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }
}
