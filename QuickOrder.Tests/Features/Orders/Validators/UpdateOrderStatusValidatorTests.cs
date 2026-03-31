using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Features.Orders.Validators;
using Xunit;

namespace QuickOrder.Tests.Features.Orders.Validators;

public class UpdateOrderStatusValidatorTests
{
    private readonly UpdateOrderStatusValidator _validator = new();

    [Theory]
    [InlineData("Pending")]
    [InlineData("InPreparation")]
    [InlineData("Ready")]
    [InlineData("Delivered")]
    [InlineData("Cancelled")]
    public void Validate_AllValidStatuses_Pass(string status)
    {
        var result = _validator.Validate(new UpdateOrderStatusCommand(1, status));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("inpreparation")]
    [InlineData("READY")]
    public void Validate_ValidStatusCaseInsensitive_Passes(string status)
    {
        var result = _validator.Validate(new UpdateOrderStatusCommand(1, status));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidStatus_Fails()
    {
        var result = _validator.Validate(new UpdateOrderStatusCommand(1, "EnCamino"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }

    [Fact]
    public void Validate_EmptyStatus_Fails()
    {
        var result = _validator.Validate(new UpdateOrderStatusCommand(1, ""));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ZeroOrderId_Fails()
    {
        var result = _validator.Validate(new UpdateOrderStatusCommand(0, "Ready"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OrderId");
    }
}
