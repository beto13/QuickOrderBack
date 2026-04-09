using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Domain.Enums;

namespace QuickOrder.Application.Features.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, string Status) : IRequest<Result>;

public class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IOrderHub hub) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Fail(Error.NotFound($"Pedido {request.OrderId} no encontrado."));

        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
            return Result.Fail(Error.Validation($"Estado '{request.Status}' no válido."));

        var fromStatus = order.Status.ToString();

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        orderRepository.AddStatusHistory(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = fromStatus,
            ToStatus = newStatus.ToString()
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await hub.NotifyOrderStatusChanged(order.Id, newStatus.ToString());

        return Result.Ok();
    }
}
