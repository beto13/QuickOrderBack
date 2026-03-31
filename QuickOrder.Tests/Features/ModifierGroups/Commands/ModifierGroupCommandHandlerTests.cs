using Moq;
using QuickOrder.Application.Features.ModifierGroups.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.ModifierGroups.Commands;

public class ModifierGroupCommandHandlerTests
{
    [Fact]
    public async Task CreateModifierGroup_ValidCommand_AddsGroupAndReturnsDto()
    {
        var repo = new Mock<IModifierGroupRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateModifierGroupCommandHandler(repo.Object, uow.Object)
            .Handle(new CreateModifierGroupCommand("Cocción", 1, 1, true, null, null), CancellationToken.None);

        Assert.Equal("Cocción", result.Name);
        Assert.Equal(1, result.MinSelections);
        Assert.Equal(1, result.MaxSelections);
        Assert.True(result.IsRequired);
        repo.Verify(r => r.Add(It.IsAny<ModifierGroup>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateModifierGroup_GroupNotFound_ThrowsKeyNotFoundException()
    {
        var repo = new Mock<IModifierGroupRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModifierGroup?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new UpdateModifierGroupCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new UpdateModifierGroupCommand(99, null, null, null, null), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateModifierGroup_OnlyUpdatesProvidedFields()
    {
        var group = new ModifierGroup { Id = 1, Name = "Original", MinSelections = 0, MaxSelections = 2, IsRequired = false };
        var repo = new Mock<IModifierGroupRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateModifierGroupCommandHandler(repo.Object, uow.Object)
            .Handle(new UpdateModifierGroupCommand(1, "Nuevo nombre", null, null, null), CancellationToken.None);

        Assert.Equal("Nuevo nombre", result.Name);
        Assert.Equal(0, result.MinSelections);
        Assert.Equal(2, result.MaxSelections);
        Assert.False(result.IsRequired);
    }

    [Fact]
    public async Task DeleteModifierGroup_GroupNotFound_ThrowsKeyNotFoundException()
    {
        var repo = new Mock<IModifierGroupRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModifierGroup?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new DeleteModifierGroupCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
                .Handle(new DeleteModifierGroupCommand(99), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteModifierGroup_ValidId_RemovesGroupAndSaves()
    {
        var group = new ModifierGroup { Id = 1, Name = "A borrar" };
        var repo = new Mock<IModifierGroupRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await new DeleteModifierGroupCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteModifierGroupCommand(1), CancellationToken.None);

        repo.Verify(r => r.Remove(group), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
