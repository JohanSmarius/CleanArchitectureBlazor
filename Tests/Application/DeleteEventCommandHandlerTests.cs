using Application;
using Application.Commands;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="DeleteEventCommandHandler"/>.
/// </summary>
public class DeleteEventCommandHandlerTests
{
    private static (DeleteEventCommandHandler handler, Mock<IEventCommandRepository> repoMock) BuildHandler()
    {
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<DeleteEventCommandHandler>>();

        repoMock
            .Setup(r => r.DeleteEventAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        return (new DeleteEventCommandHandler(repoMock.Object, loggerMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryDeleteOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();
        var command = new DeleteEventCommand { Id = 42 };

        // Act
        await handler.Handle(command);

        // Assert
        repoMock.Verify(r => r.DeleteEventAsync(42), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCorrectIdToRepository()
    {
        // Arrange
        int? capturedId = null;
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<DeleteEventCommandHandler>>();

        repoMock
            .Setup(r => r.DeleteEventAsync(It.IsAny<int>()))
            .Callback<int>(id => capturedId = id)
            .Returns(Task.CompletedTask);

        var handler = new DeleteEventCommandHandler(repoMock.Object, loggerMock.Object);
        var command = new DeleteEventCommand { Id = 99 };

        // Act
        await handler.Handle(command);

        // Assert
        Assert.Equal(99, capturedId);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<DeleteEventCommandHandler>>();

        repoMock
            .Setup(r => r.DeleteEventAsync(It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException("Event not found"));

        var handler = new DeleteEventCommandHandler(repoMock.Object, loggerMock.Object);
        var command = new DeleteEventCommand { Id = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_ValidCommand_CompletesWithoutException()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = new DeleteEventCommand { Id = 1 };

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(command));

        // Assert
        Assert.Null(exception);
    }
}
