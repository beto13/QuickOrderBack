using MediatR;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Domain.Enums;

namespace QuickOrder.Application.Features.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, string Status) : IRequest;

public class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IOrderHub hub) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Pedido {request.OrderId} no encontrado.");

        if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Estado '{request.Status}' no válido.");

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
    }
}
