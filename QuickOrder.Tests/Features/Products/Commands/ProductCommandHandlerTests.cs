using Moq;
using QuickOrder.Application.Features.Products.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Products.Commands;

public class ProductCommandHandlerTests
{
    [Fact]
    public async Task CreateProduct_ValidCommand_AddsProductAndReturnsDto()
    {
        var productRepo = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateProductCommandHandler(productRepo.Object, unitOfWork.Object)
            .Handle(new CreateProductCommand("Milanesa napolitana", "Con salsa y mozzarella"), CancellationToken.None);

        Assert.Equal("Milanesa napolitana", result.Name);
        Assert.Equal("Con salsa y mozzarella", result.Description);
        productRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new UpdateProductCommandHandler(productRepo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new UpdateProductCommand(99, "Nuevo nombre", null), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProduct_OnlyUpdatesProvidedFields()
    {
        var product = new Product { Id = 1, Name = "Original", Description = "Desc original" };
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateProductCommandHandler(productRepo.Object, unitOfWork.Object)
            .Handle(new UpdateProductCommand(1, "Nuevo nombre", null), CancellationToken.None);

        Assert.Equal("Nuevo nombre", result.Name);
        Assert.Equal("Desc original", result.Description); // no cambió
    }

    [Fact]
    public async Task DeleteProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new DeleteProductCommandHandler(productRepo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new DeleteProductCommand(99), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteProduct_ValidId_RemovesProductAndSaves()
    {
        var product = new Product { Id = 1, Name = "A borrar" };
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await new DeleteProductCommandHandler(productRepo.Object, unitOfWork.Object)
            .Handle(new DeleteProductCommand(1), CancellationToken.None);

        productRepo.Verify(r => r.Remove(product), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
