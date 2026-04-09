using Moq;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Features.Orders.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Application.Messages;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Orders.Commands;

public class CreateOrderCommandHandlerInvalidProductTests
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
    public async Task Handle_MenuProductIdNotInDictionary_ThrowsKeyNotFoundException()
    {
        // Arrange - table found, but the requested MenuProductId (99) is not in the result
        var table = new Table { Id = 1, Number = "3" };
        _tableRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        _menuProductRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, MenuProduct>()); // empty — product 99 not available/found

        var command = new CreateOrderCommand(1, 2, null, [new CreateOrderItemRequest(99, 1, null)]);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));

        _orderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleItems_OneInvalidMenuProduct_ThrowsKeyNotFoundException()
    {
        // Arrange - only product 10 is available, product 20 is not
        var table = new Table { Id = 1, Number = "2" };
        _tableRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        var product = new Product { Id = 1, Name = "Pizza" };
        var menuProduct = new MenuProduct { Id = 10, ProductId = 1, MenuId = 2, Price = 500m, Product = product };

        _menuProductRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, MenuProduct> { [10] = menuProduct }); // product 20 missing

        var command = new CreateOrderCommand(1, 2, null, [
            new CreateOrderItemRequest(10, 1, null),
            new CreateOrderItemRequest(20, 1, null) // not available
        ]);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));

        _orderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MenuProductWithNullMenu_ReturnsEmptyMenuName()
    {
        // Arrange - MenuProduct has null Menu navigation (Menu?.Name ?? string.Empty)
        var table = new Table { Id = 1, Number = "1" };
        _tableRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);

        var product = new Product { Id = 1, Name = "Hamburguesa" };
        var menuProduct = new MenuProduct { Id = 5, ProductId = 1, MenuId = 3, Price = 800m, Product = product, Menu = null };

        _menuProductRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, MenuProduct> { [5] = menuProduct });

        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _hub.Setup(h => h.NotifyNewOrder(It.IsAny<OrderDto>())).Returns(Task.CompletedTask);
        _publisher.Setup(p => p.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateOrderCommand(1, 3, null, [new CreateOrderItemRequest(5, 2, null)]);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert — should not throw, menu name defaults to empty string
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.MenuName);
    }
}
