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

        Assert.True(result.IsSuccess);
        Assert.Equal("Milanesa napolitana", result.Value!.Name);
        Assert.Equal("Con salsa y mozzarella", result.Value!.Description);
        productRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ProductNotFound_ReturnsNotFoundError()
    {
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await new UpdateProductCommandHandler(productRepo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new UpdateProductCommand(99, "Nuevo nombre", null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
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

        Assert.True(result.IsSuccess);
        Assert.Equal("Nuevo nombre", result.Value!.Name);
        Assert.Equal("Desc original", result.Value!.Description); // no cambió
    }

    [Fact]
    public async Task DeleteProduct_ProductNotFound_ReturnsNotFoundError()
    {
        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await new DeleteProductCommandHandler(productRepo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new DeleteProductCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
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

        var result = await new DeleteProductCommandHandler(productRepo.Object, unitOfWork.Object)
            .Handle(new DeleteProductCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        productRepo.Verify(r => r.Remove(product), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
