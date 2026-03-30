using QuickOrder.Application.Features.Products.Commands;
using QuickOrder.Application.Features.Products.Validators;

namespace QuickOrder.Tests.Features.Products.Validators;

public class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var result = _validator.Validate(new CreateProductCommand("Pizza", "Rica pizza"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyName_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand("", null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand(new string('x', 201), null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_DescriptionTooLong_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand("Pizza", new string('x', 501)));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }
}
