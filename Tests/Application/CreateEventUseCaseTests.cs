using Application;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="CreateEventUseCase"/>.
/// </summary>
public class CreateEventUseCaseTests
{
    private static EventDTO BuildValidDTO() => new()
    {
        Name = "Festival Medical Cover",
        StartDate = DateTime.UtcNow.AddDays(2),
        EndDate = DateTime.UtcNow.AddDays(3),
        Location = "Festival Grounds",
    };

    private static (CreateEventUseCase useCase, Mock<IEventRepository> repoMock) BuildUseCase(Event? returnedEvent = null)
    {
        var repoMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventService>>();

        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        return (new CreateEventUseCase(repoMock.Object, loggerMock.Object), repoMock);
    }

    [Fact]
    public async Task Execute_StartDateEqualToEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _) = BuildUseCase();
        var dto = BuildValidDTO();
        dto.EndDate = dto.StartDate;

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(dto));
    }

    [Fact]
    public async Task Execute_StartDateAfterEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _) = BuildUseCase();
        var dto = BuildValidDTO();
        dto.EndDate = dto.StartDate.AddHours(-1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(dto));
    }

    [Fact]
    public async Task Execute_StartDateInThePast_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _) = BuildUseCase();
        var dto = BuildValidDTO();
        dto.StartDate = DateTime.UtcNow.AddDays(-1);
        dto.EndDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(dto));
    }

    [Fact]
    public async Task Execute_ValidEvent_ReturnsEventDTO()
    {
        // Arrange
        var (useCase, _) = BuildUseCase();
        var dto = BuildValidDTO();

        // Act
        var result = await useCase.Execute(dto);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Execute_ValidEvent_SetsStatusToRequested()
    {
        // Arrange
        var (useCase, _) = BuildUseCase();
        var dto = BuildValidDTO();

        // Act
        var result = await useCase.Execute(dto);

        // Assert
        Assert.Equal(EventStatusDTO.Requested, result.Status);
    }

    [Fact]
    public async Task Execute_ValidEvent_CallsRepositoryCreateOnce()
    {
        // Arrange
        var (useCase, repoMock) = BuildUseCase();
        var dto = BuildValidDTO();

        // Act
        await useCase.Execute(dto);

        // Assert
        repoMock.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ValidEvent_AddsDefaultShift()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventService>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var useCase = new CreateEventUseCase(repoMock.Object, loggerMock.Object);
        var dto = BuildValidDTO();

        // Act
        await useCase.Execute(dto);

        // Assert
        Assert.Single(capturedEntity!.Shifts);
    }

    [Fact]
    public async Task Execute_ValidEvent_DefaultShiftNameIsDefaultShift()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventService>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var useCase = new CreateEventUseCase(repoMock.Object, loggerMock.Object);
        var dto = BuildValidDTO();

        // Act
        await useCase.Execute(dto);

        // Assert
        Assert.Equal("Default Shift", capturedEntity!.Shifts[0].Name);
    }

    [Fact]
    public async Task Execute_ValidEvent_DefaultShiftCoversEntireEventDuration()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventService>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var useCase = new CreateEventUseCase(repoMock.Object, loggerMock.Object);
        var dto = BuildValidDTO();

        // Act
        await useCase.Execute(dto);

        // Assert
        var shift = capturedEntity!.Shifts[0];
        Assert.Equal(dto.StartDate, shift.StartTime);
        Assert.Equal(dto.EndDate, shift.EndTime);
    }
}
