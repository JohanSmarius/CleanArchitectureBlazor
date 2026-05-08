using Application;
using Application.Queries;
using Entities;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="GetAllEventsQueryHandler"/>.
/// </summary>
public class GetAllEventsQueryHandlerTests
{
    private static (GetAllEventsQueryHandler handler, Mock<IEventQueryRepository> repoMock) BuildHandler(
        List<Event>? events = null)
    {
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetAllEventsAsync())
            .ReturnsAsync(events ?? new List<Event>());

        return (new GetAllEventsQueryHandler(repoMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_ReturnsAllEventsFromRepository()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Id = 1, Name = "Event A", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(2), Location = "Location A" },
            new() { Id = 2, Name = "Event B", StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(4), Location = "Location B" },
        };

        var (handler, _) = BuildHandler(events);

        // Act
        var result = await handler.Handle(new GetAllEventsQuery());

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var (handler, _) = BuildHandler(new List<Event>());

        // Act
        var result = await handler.Handle(new GetAllEventsQuery());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetAllEventsOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();

        // Act
        await handler.Handle(new GetAllEventsQuery());

        // Assert
        repoMock.Verify(r => r.GetAllEventsAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsExactRepositoryResult()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Id = 5, Name = "Event 5", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(2), Location = "Loc" },
        };

        var (handler, _) = BuildHandler(events);

        // Act
        var result = await handler.Handle(new GetAllEventsQuery());

        // Assert
        Assert.Same(events, result);
    }
}
