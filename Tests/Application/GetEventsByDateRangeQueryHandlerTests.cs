using Application;
using Application.Queries;
using Entities;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="GetEventsByDateRangeQueryHandler"/>.
/// </summary>
public class GetEventsByDateRangeQueryHandlerTests
{
    private static (GetEventsByDateRangeQueryHandler handler, Mock<IEventQueryRepository> repoMock) BuildHandler(
        List<Event>? events = null)
    {
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetEventsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(events ?? new List<Event>());

        return (new GetEventsByDateRangeQueryHandler(repoMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_ReturnsEventsFromRepository()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(10);

        var events = new List<Event>
        {
            new() { Id = 1, Name = "Event A", StartDate = start.AddDays(1), EndDate = start.AddDays(2), Location = "Venue A" },
            new() { Id = 2, Name = "Event B", StartDate = start.AddDays(3), EndDate = start.AddDays(4), Location = "Venue B" },
        };

        var (handler, _) = BuildHandler(events);

        // Act
        var result = await handler.Handle(new GetEventsByDateRangeQuery { StartDate = start, EndDate = end });

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WhenNoEventsInRange_ReturnsEmptyList()
    {
        // Arrange
        var (handler, _) = BuildHandler(new List<Event>());

        // Act
        var result = await handler.Handle(new GetEventsByDateRangeQuery
        {
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        });

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_PassesCorrectStartDateToRepository()
    {
        // Arrange
        DateTime? capturedStart = null;
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetEventsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Callback<DateTime, DateTime>((s, _) => capturedStart = s)
            .ReturnsAsync(new List<Event>());

        var handler = new GetEventsByDateRangeQueryHandler(repoMock.Object);
        var expectedStart = DateTime.UtcNow.AddDays(1);

        // Act
        await handler.Handle(new GetEventsByDateRangeQuery { StartDate = expectedStart, EndDate = expectedStart.AddDays(5) });

        // Assert
        Assert.Equal(expectedStart, capturedStart);
    }

    [Fact]
    public async Task Handle_PassesCorrectEndDateToRepository()
    {
        // Arrange
        DateTime? capturedEnd = null;
        var repoMock = new Mock<IEventQueryRepository>();

        repoMock
            .Setup(r => r.GetEventsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Callback<DateTime, DateTime>((_, e) => capturedEnd = e)
            .ReturnsAsync(new List<Event>());

        var handler = new GetEventsByDateRangeQueryHandler(repoMock.Object);
        var expectedEnd = DateTime.UtcNow.AddDays(7);

        // Act
        await handler.Handle(new GetEventsByDateRangeQuery { StartDate = expectedEnd.AddDays(-5), EndDate = expectedEnd });

        // Assert
        Assert.Equal(expectedEnd, capturedEnd);
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetEventsByDateRangeOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();

        // Act
        await handler.Handle(new GetEventsByDateRangeQuery
        {
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5)
        });

        // Assert
        repoMock.Verify(r => r.GetEventsByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsExactRepositoryResult()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Id = 10, Name = "Range Event", StartDate = DateTime.UtcNow.AddDays(2), EndDate = DateTime.UtcNow.AddDays(3), Location = "Arena" },
        };

        var (handler, _) = BuildHandler(events);

        // Act
        var result = await handler.Handle(new GetEventsByDateRangeQuery
        {
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5)
        });

        // Assert
        Assert.Same(events, result);
    }
}
