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

        Assert.Equal("Entradas", result.Name);
        Assert.Equal(1, result.DisplayOrder);
        repo.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new UpdateCategoryCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new UpdateCategoryCommand(99, null, null), CancellationToken.None));
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

        Assert.Equal("Nuevo nombre", result.Name);
        Assert.Equal(0, result.DisplayOrder);
    }

    [Fact]
    public async Task DeleteCategory_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new DeleteCategoryCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new DeleteCategoryCommand(99), CancellationToken.None));
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

        await new DeleteCategoryCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        repo.Verify(r => r.Remove(category), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
