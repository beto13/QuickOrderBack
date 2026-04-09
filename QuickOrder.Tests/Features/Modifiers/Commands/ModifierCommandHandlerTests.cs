using Moq;
using QuickOrder.Application.Features.Modifiers.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Modifiers.Commands;

public class ModifierCommandHandlerTests
{
    [Fact]
    public async Task CreateModifier_ValidCommand_AddsModifierAndReturnsDto()
    {
        var group = new ModifierGroup { Id = 1, Name = "Cocción" };
        var groupRepo = new Mock<IModifierGroupRepository>();
        groupRepo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        var modifierRepo = new Mock<IModifierRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateModifierCommandHandler(groupRepo.Object, modifierRepo.Object, uow.Object)
            .Handle(new CreateModifierCommand(1, "A punto", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("A punto", result.Value!.Name);
        Assert.Equal(1, result.Value!.ModifierGroupId);
        modifierRepo.Verify(r => r.Add(It.IsAny<Modifier>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateModifier_GroupNotFound_ReturnsNotFoundError()
    {
        var groupRepo = new Mock<IModifierGroupRepository>();
        groupRepo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModifierGroup?)null);

        var result = await new CreateModifierCommandHandler(groupRepo.Object, Mock.Of<IModifierRepository>(), Mock.Of<IUnitOfWork>())
            .Handle(new CreateModifierCommand(99, "A punto", null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task UpdateModifier_ModifierNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<IModifierRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Modifier?)null);

        var result = await new UpdateModifierCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new UpdateModifierCommand(99, null, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task UpdateModifier_OnlyUpdatesProvidedFields()
    {
        var modifier = new Modifier { Id = 1, ModifierGroupId = 1, Name = "Original", Description = "Desc original" };
        var repo = new Mock<IModifierRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(modifier);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateModifierCommandHandler(repo.Object, uow.Object)
            .Handle(new UpdateModifierCommand(1, "Bien cocido", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Bien cocido", result.Value!.Name);
        Assert.Equal("Desc original", result.Value!.Description);
    }

    [Fact]
    public async Task DeleteModifier_ModifierNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<IModifierRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Modifier?)null);

        var result = await new DeleteModifierCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new DeleteModifierCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task DeleteModifier_ValidId_RemovesModifierAndSaves()
    {
        var modifier = new Modifier { Id = 1, Name = "A borrar" };
        var repo = new Mock<IModifierRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(modifier);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new DeleteModifierCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteModifierCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repo.Verify(r => r.Remove(modifier), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
