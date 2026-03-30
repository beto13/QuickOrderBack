using FluentValidation;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Domain.Enums;

namespace QuickOrder.Application.Features.Orders.Validators;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    private static readonly string[] ValidStatuses =
        Enum.GetNames<OrderStatus>();

    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("El id del pedido es requerido.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("El estado es requerido.")
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Estado no válido. Los valores permitidos son: {string.Join(", ", ValidStatuses)}.");
    }
}
