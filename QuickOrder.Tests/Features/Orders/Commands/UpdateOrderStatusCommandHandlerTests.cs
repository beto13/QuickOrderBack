using Moq;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Domain.Enums;

namespace QuickOrder.Tests.Features.Orders.Commands;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOrderHub> _hub = new();

    private UpdateOrderStatusCommandHandler CreateHandler() => new(
        _orderRepo.Object, _unitOfWork.Object, _hub.Object);

    [Fact]
    public async Task Handle_ValidStatus_UpdatesOrderStatusAndNotifiesHub()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _hub.Setup(h => h.NotifyOrderStatusChanged(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await CreateHandler().Handle(new UpdateOrderStatusCommand(1, "InPreparation"), CancellationToken.None);

        Assert.Equal(OrderStatus.InPreparation, order.Status);
        _orderRepo.Verify(r => r.AddStatusHistory(It.Is<OrderStatusHistory>(h =>
            h.OrderId == 1 &&
            h.FromStatus == "Pending" &&
            h.ToStatus == "InPreparation")), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _hub.Verify(h => h.NotifyOrderStatusChanged(1, "InPreparation"), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsKeyNotFoundException()
    {
        _orderRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateHandler().Handle(new UpdateOrderStatusCommand(99, "Ready"), CancellationToken.None));

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ThrowsArgumentException()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            CreateHandler().Handle(new UpdateOrderStatusCommand(1, "EstadoInvalido"), CancellationToken.None));

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
