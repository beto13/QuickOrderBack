using Moq;
using QuickOrder.Application.Features.Categories.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Categories.Commands;

public class CategoryCommandHandlerTests
{
    [Fact]
    public async Task CreateCategory_ValidCommand_AddsCategoryAndReturnsDto()
    {
        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateCategoryCommandHandler(repo.Object, uow.Object)
            .Handle(new CreateCategoryCommand("Entradas", 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Entradas", result.Value!.Name);
        Assert.Equal(1, result.Value!.DisplayOrder);
        repo.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CategoryNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await new UpdateCategoryCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new UpdateCategoryCommand(99, null, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task UpdateCategory_OnlyUpdatesProvidedFields()
    {
        var category = new Category { Id = 1, Name = "Original", DisplayOrder = 0 };
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateCategoryCommandHandler(repo.Object, uow.Object)
            .Handle(new UpdateCategoryCommand(1, "Nuevo nombre", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nuevo nombre", result.Value!.Name);
        Assert.Equal(0, result.Value!.DisplayOrder);
    }

    [Fact]
    public async Task DeleteCategory_CategoryNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await new DeleteCategoryCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new DeleteCategoryCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task DeleteCategory_ValidId_RemovesCategoryAndSaves()
    {
        var category = new Category { Id = 1, Name = "A borrar" };
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new DeleteCategoryCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repo.Verify(r => r.Remove(category), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
