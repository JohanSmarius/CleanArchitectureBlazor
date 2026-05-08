using Application;
using Application.Queries;
using Entities;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="GetUpcomingEventsQueryHandler"/>.
/// </summary>
public class GetUpcomingEventsQueryHandlerTests
{
    private static (GetUpcomingEventsQueryHandler handler, Mock<IEventQueryRepository> repoMock) BuildHandler(
        List<Event>? events = null)
    {
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetUpcomingEventsAsync())
            .ReturnsAsync(events ?? new List<Event>());

        return (new GetUpcomingEventsQueryHandler(repoMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_ReturnsUpcomingEventsFromRepository()
    {
        // Arrange
        var upcoming = new List<Event>
        {
            new() { Id = 1, Name = "Upcoming Event", StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(6), Location = "Venue" },
        };

        var (handler, _) = BuildHandler(upcoming);

        // Act
        var result = await handler.Handle(new GetUpcomingEventsQuery());

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_WhenNoUpcomingEvents_ReturnsEmptyList()
    {
        // Arrange
        var (handler, _) = BuildHandler(new List<Event>());

        // Act
        var result = await handler.Handle(new GetUpcomingEventsQuery());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetUpcomingEventsOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();

        // Act
        await handler.Handle(new GetUpcomingEventsQuery());

        // Assert
        repoMock.Verify(r => r.GetUpcomingEventsAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsExactRepositoryResult()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Id = 3, Name = "Future Event", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(2), Location = "Hall" },
        };

        var (handler, _) = BuildHandler(events);

        // Act
        var result = await handler.Handle(new GetUpcomingEventsQuery());

        // Assert
        Assert.Same(events, result);
    }
}
