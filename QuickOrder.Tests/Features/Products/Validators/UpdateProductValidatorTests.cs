using QuickOrder.Application.Features.Products.Commands;
using QuickOrder.Application.Features.Products.Validators;

namespace QuickOrder.Tests.Features.Products.Validators;

public class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_WithNullOptionals_Passes()
    {
        var result = _validator.Validate(new UpdateProductCommand(1, null, null));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ValidCommand_WithValues_Passes()
    {
        var result = _validator.Validate(new UpdateProductCommand(1, "Nueva Pizza", "Descripción nueva"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_IdZero_Fails()
    {
        var result = _validator.Validate(new UpdateProductCommand(0, "Pizza", null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_NameTooLong_Fails()
    {
        var result = _validator.Validate(new UpdateProductCommand(1, new string('x', 201), null));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NullName_Passes()
    {
        // null Name is allowed in an update (partial update)
        var result = _validator.Validate(new UpdateProductCommand(1, null, null));
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_DescriptionTooLong_Fails()
    {
        var result = _validator.Validate(new UpdateProductCommand(1, null, new string('x', 501)));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_NullDescription_Passes()
    {
        var result = _validator.Validate(new UpdateProductCommand(1, "Pizza", null));
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Description");
    }
}
