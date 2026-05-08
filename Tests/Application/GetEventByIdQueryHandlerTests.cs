using Application;
using Application.Queries;
using Entities;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="GetEventByIdQueryHandler"/>.
/// </summary>
public class GetEventByIdQueryHandlerTests
{
    private static (GetEventByIdQueryHandler handler, Mock<IEventQueryRepository> repoMock) BuildHandler(
        Event? eventToReturn = null)
    {
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(eventToReturn);

        return (new GetEventByIdQueryHandler(repoMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsEvent()
    {
        // Arrange
        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Location"
        };

        var (handler, _) = BuildHandler(existingEvent);

        // Act
        var result = await handler.Handle(new GetEventByIdQuery { Id = 1 });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsCorrectEvent()
    {
        // Arrange
        var existingEvent = new Event
        {
            Id = 7,
            Name = "Medical Cover",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Stadium"
        };

        var (handler, _) = BuildHandler(existingEvent);

        // Act
        var result = await handler.Handle(new GetEventByIdQuery { Id = 7 });

        // Assert
        Assert.Equal(7, result!.Id);
        Assert.Equal("Medical Cover", result.Name);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        // Arrange
        var (handler, _) = BuildHandler(null);

        // Act
        var result = await handler.Handle(new GetEventByIdQuery { Id = 999 });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_PassesCorrectIdToRepository()
    {
        // Arrange
        int? capturedId = null;
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .Callback<int>(id => capturedId = id)
            .ReturnsAsync((Event?)null);

        var handler = new GetEventByIdQueryHandler(repoMock.Object);

        // Act
        await handler.Handle(new GetEventByIdQuery { Id = 42 });

        // Assert
        Assert.Equal(42, capturedId);
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetEventByIdOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();

        // Act
        await handler.Handle(new GetEventByIdQuery { Id = 1 });

        // Assert
        repoMock.Verify(r => r.GetEventByIdAsync(1), Times.Once);
    }
}
