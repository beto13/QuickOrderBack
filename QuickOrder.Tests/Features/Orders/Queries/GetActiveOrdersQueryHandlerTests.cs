using Moq;
using QuickOrder.Application.Features.Orders.Queries;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Domain.Enums;

namespace QuickOrder.Tests.Features.Orders.Queries;

public class GetActiveOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsActiveOrdersMappedToDto()
    {
        var product = new Product { Id = 1, Name = "Milanesa" };
        var menuProduct = new MenuProduct { Id = 1, Product = product };
        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                TableId = 1,
                MenuId = 1,
                Status = OrderStatus.Pending,
                Table = new Table { Id = 1, Number = "3" },
                Menu = new Menu { Id = 1, Name = "Salón" },
                Items = [new OrderItem { MenuProductId = 1, Quantity = 2, UnitPrice = 800m, MenuProduct = menuProduct }],
                StatusHistory = []
            }
        };

        var orderRepo = new Mock<IOrderRepository>();
        orderRepo.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var result = await new GetActiveOrdersQueryHandler(orderRepo.Object)
            .Handle(new GetActiveOrdersQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("3", result[0].TableNumber);
        Assert.Equal("Salón", result[0].MenuName);
        Assert.Equal("Pending", result[0].Status);
        Assert.Equal(800m, result[0].Items[0].UnitPrice);
        Assert.Equal(2, result[0].Items[0].Quantity);
    }

    [Fact]
    public async Task Handle_NoActiveOrders_ReturnsEmptyList()
    {
        var orderRepo = new Mock<IOrderRepository>();
        orderRepo.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await new GetActiveOrdersQueryHandler(orderRepo.Object)
            .Handle(new GetActiveOrdersQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
