using Moq;
using QuickOrder.Application.Features.Tables.Commands;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Tests.Features.Tables.Commands;

public class TableCommandHandlerTests
{
    [Fact]
    public async Task CreateTable_ValidCommand_AddsTableAndReturnsDto()
    {
        var repo = new Mock<ITableRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new CreateTableCommandHandler(repo.Object, uow.Object)
            .Handle(new CreateTableCommand("5", 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("5", result.Value!.Number);
        Assert.True(result.Value!.IsActive);
        repo.Verify(r => r.Add(It.IsAny<Table>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTable_TableNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<ITableRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        var result = await new UpdateTableCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new UpdateTableCommand(99, null, null, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task UpdateTable_OnlyUpdatesProvidedFields()
    {
        var table = new Table { Id = 1, Number = "3", IsActive = true };
        var repo = new Mock<ITableRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new UpdateTableCommandHandler(repo.Object, uow.Object)
            .Handle(new UpdateTableCommand(1, "10", null, null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("10", result.Value!.Number);
        Assert.True(result.Value!.IsActive);
    }

    [Fact]
    public async Task DeleteTable_TableNotFound_ReturnsNotFoundError()
    {
        var repo = new Mock<ITableRepository>();
        repo.Setup(r => r.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Table?)null);

        var result = await new DeleteTableCommandHandler(repo.Object, Mock.Of<IUnitOfWork>())
            .Handle(new DeleteTableCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task DeleteTable_ValidId_DeactivatesTableAndSaves()
    {
        var table = new Table { Id = 1, Number = "5", IsActive = true };
        var repo = new Mock<ITableRepository>();
        repo.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new DeleteTableCommandHandler(repo.Object, uow.Object)
            .Handle(new DeleteTableCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(table.IsActive);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
