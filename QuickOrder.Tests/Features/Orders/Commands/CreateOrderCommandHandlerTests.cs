using Moq;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Application.Messages;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<ITableRepository> _tableRepo = new();
    private readonly Mock<IMenuProductRepository> _menuProductRepo = new();
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOrderHub> _hub = new();
    private readonly Mock<IMessagePublisher> _publisher = new();

    private CreateOrderCommandHandler CreateHandler() => new(
        _tableRepo.Object, _menuProductRepo.Object, _orderRepo.Object,
        _unitOfWork.Object, _hub.Object, _publisher.Object);

    [Fact]
    public async Task Handle_ValidCommand_CreatesOrderAndReturnsDto()
    {
        // Arrange
        var table = new Table { Id = 1, Number = "5" };
        _tableRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        var product = new Product { Id = 1, Name = "Pizza Napolitana" };
        var menuProduct = new MenuProduct { Id = 10, ProductId = 1, MenuId = 2, Price = 1500m, Product = product };
        _menuProductRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, MenuProduct> { [10] = menuProduct });

        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _hub.Setup(h => h.NotifyNewOrder(It.IsAny<OrderDto>())).Returns(Task.CompletedTask);
        _publisher.Setup(p => p.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateOrderCommand(1, 2, "Sin sal", [new CreateOrderItemRequest(10, 2, null)]);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TableId);
        Assert.Equal("5", result.TableNumber);
        Assert.Single(result.Items);
        Assert.Equal(1500m, result.Items[0].UnitPrice);
        _orderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _hub.Verify(h => h.NotifyNewOrder(It.IsAny<OrderDto>()), Times.Once);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TableNotFound_ThrowsKeyNotFoundException()
    {
        _tableRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        var command = new CreateOrderCommand(99, 1, null, [new CreateOrderItemRequest(1, 1, null)]);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));

        _orderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
