using Application;
using Application.Commands;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="CreateEventCommandHandler"/>.
/// </summary>
public class CreateEventCommandHandlerTests
{
    private static CreateEventCommand BuildValidCommand() => new()
    {
        Name = "Festival Medical Cover",
        StartDate = DateTime.UtcNow.AddDays(2),
        EndDate = DateTime.UtcNow.AddDays(3),
        Location = "Festival Grounds",
    };

    private static (CreateEventCommandHandler handler, Mock<IEventCommandRepository> repoMock) BuildHandler()
    {
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<CreateEventCommandHandler>>();

        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        return (new CreateEventCommandHandler(repoMock.Object, loggerMock.Object), repoMock);
    }

    [Fact]
    public async Task Handle_StartDateEqualToEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = BuildValidCommand();
        command.EndDate = command.StartDate;

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_StartDateAfterEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = BuildValidCommand();
        command.EndDate = command.StartDate.AddHours(-1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_StartDateInThePast_ThrowsApplicationException()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = BuildValidCommand();
        command.StartDate = DateTime.UtcNow.AddDays(-1);
        command.EndDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsEventDTO()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = BuildValidCommand();

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsStatusToRequested()
    {
        // Arrange
        var (handler, _) = BuildHandler();
        var command = BuildValidCommand();

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.Equal(EventStatusDTO.Requested, result.Status);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryCreateOnce()
    {
        // Arrange
        var (handler, repoMock) = BuildHandler();
        var command = BuildValidCommand();

        // Act
        await handler.Handle(command);

        // Assert
        repoMock.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsDefaultShift()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<CreateEventCommandHandler>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var handler = new CreateEventCommandHandler(repoMock.Object, loggerMock.Object);
        var command = BuildValidCommand();

        // Act
        await handler.Handle(command);

        // Assert
        Assert.Single(capturedEntity!.Shifts);
    }

    [Fact]
    public async Task Handle_ValidCommand_DefaultShiftNameIsDefaultShift()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<CreateEventCommandHandler>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var handler = new CreateEventCommandHandler(repoMock.Object, loggerMock.Object);
        var command = BuildValidCommand();

        // Act
        await handler.Handle(command);

        // Assert
        Assert.Equal("Default Shift", capturedEntity!.Shifts[0].Name);
    }

    [Fact]
    public async Task Handle_ValidCommand_DefaultShiftCoversEntireEventDuration()
    {
        // Arrange
        Event? capturedEntity = null;
        var repoMock = new Mock<IEventCommandRepository>();
        var loggerMock = new Mock<ILogger<CreateEventCommandHandler>>();
        repoMock
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEntity = e)
            .ReturnsAsync((Event e) => e);

        var handler = new CreateEventCommandHandler(repoMock.Object, loggerMock.Object);
        var command = BuildValidCommand();

        // Act
        await handler.Handle(command);

        // Assert
        var shift = capturedEntity!.Shifts[0];
        Assert.Equal(command.StartDate, shift.StartTime);
        Assert.Equal(command.EndDate, shift.EndTime);
    }
}
