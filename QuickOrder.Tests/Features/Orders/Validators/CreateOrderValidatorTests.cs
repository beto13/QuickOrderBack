using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Features.Orders.Validators;

namespace QuickOrder.Tests.Features.Orders.Validators;

public class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var command = new CreateOrderCommand(1, 1, null, [new CreateOrderItemRequest(1, 2, null)]);
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_TableIdZero_Fails()
    {
        var command = new CreateOrderCommand(0, 1, null, [new CreateOrderItemRequest(1, 1, null)]);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TableId");
    }

    [Fact]
    public void Validate_MenuIdZero_Fails()
    {
        var command = new CreateOrderCommand(1, 0, null, [new CreateOrderItemRequest(1, 1, null)]);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MenuId");
    }

    [Fact]
    public void Validate_EmptyItems_Fails()
    {
        var command = new CreateOrderCommand(1, 1, null, []);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Items");
    }

    [Fact]
    public void Validate_ItemWithZeroQuantity_Fails()
    {
        var command = new CreateOrderCommand(1, 1, null, [new CreateOrderItemRequest(1, 0, null)]);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ItemWithZeroMenuProductId_Fails()
    {
        var command = new CreateOrderCommand(1, 1, null, [new CreateOrderItemRequest(0, 1, null)]);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NotesTooLong_Fails()
    {
        var command = new CreateOrderCommand(1, 1, new string('x', 501), [new CreateOrderItemRequest(1, 1, null)]);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Notes");
    }
}
