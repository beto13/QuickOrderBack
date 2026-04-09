using Moq;
using QuickOrder.Application.Features.Menus.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Menus.Commands;

public class MenuCommandHandlerTests
{
    [Fact]
    public async Task CreateMenu_ValidCommand_AddsMenuAndReturnsDto()
    {
        var repo = new Mock<IMenuRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateMenuCommandHandler(repo.Object, uow.Object)
            .Handle(new CreateMenuCommand("Salón"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Salón", result.Value!.Name);
        Assert.True(result.Value!.IsActive);
        repo.Verify(r => r.Add(It.IsAny<Menu>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMenu_MenuNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<IMenuRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Menu?)null);

        var result = await new UpdateMenuCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new UpdateMenuCommand(99, null, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task UpdateMenu_OnlyUpdatesProvidedFields()
    {
        var menu = new Menu { Id = 1, Name = "Original", IsActive = true };
        var repo = new Mock<IMenuRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menu);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateMenuCommandHandler(repo.Object, uow.Object)
            .Handle(new UpdateMenuCommand(1, "Nuevo nombre", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nuevo nombre", result.Value!.Name);
        Assert.True(result.Value!.IsActive);
    }

    [Fact]
    public async Task DeleteMenu_MenuNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<IMenuRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Menu?)null);

        var result = await new DeleteMenuCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new DeleteMenuCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task DeleteMenu_ValidId_DeactivatesMenuAndSaves()
    {
        var menu = new Menu { Id = 1, Name = "Salón", IsActive = true };
        var repo = new Mock<IMenuRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menu);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new DeleteMenuCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteMenuCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(menu.IsActive);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
