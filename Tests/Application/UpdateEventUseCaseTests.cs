using Application;
using Application.Commands;
using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="UpdateEventCommandHandler"/>.
/// </summary>
public class UpdateEventUseCaseTests
{
    private static Event CreateEvent(int id = 1, EventStatus status = EventStatus.Requested, string? email = "contact@example.com") => new()
    {
        Id = id,
        Name = "Original Name",
        StartDate = DateTime.UtcNow.AddDays(1),
        EndDate = DateTime.UtcNow.AddDays(2),
        Location = "Original Location",
        Status = status,
        ContactEmail = email,
        NotificationSent = false,
        Shifts = new List<Shift>()
    };

    private static UpdateEventCommand CreateCommand(Event source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        StartDate = source.StartDate,
        EndDate = source.EndDate,
        Location = source.Location,
        Description = source.Description,
        Status = (EventStatusDTO)source.Status,
        ContactPerson = source.ContactPerson,
        ContactPhone = source.ContactPhone,
        ContactEmail = source.ContactEmail,
        Shifts = new List<ShiftDTO>()
    };

    private static (UpdateEventCommandHandler handler, Mock<IEventQueryRepository> queryMock, Mock<IEventCommandRepository> commandMock, Mock<IEmailService> emailMock)
        BuildHandler(Event? existingEvent = null)
    {
        var queryMock = new Mock<IEventQueryRepository>();
        var commandMock = new Mock<IEventCommandRepository>();
        var emailMock = new Mock<IEmailService>();
        var loggerMock = new Mock<ILogger<UpdateEventCommandHandler>>();

        queryMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(existingEvent ?? CreateEvent());

        commandMock
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        emailMock
            .Setup(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        emailMock
            .Setup(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        return (new UpdateEventCommandHandler(queryMock.Object, commandMock.Object, emailMock.Object, loggerMock.Object),
                queryMock, commandMock, emailMock);
    }

    [Fact]
    public async Task Handle_StartDateEqualToEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (handler, _, _, _) = BuildHandler();
        var command = CreateCommand(CreateEvent());
        command.EndDate = command.StartDate;

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_StartDateAfterEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (handler, _, _, _) = BuildHandler();
        var command = CreateCommand(CreateEvent());
        command.EndDate = command.StartDate.AddHours(-1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_EventNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var queryMock = new Mock<IEventQueryRepository>();
        var commandMock = new Mock<IEventCommandRepository>();
        var emailMock = new Mock<IEmailService>();
        var loggerMock = new Mock<ILogger<UpdateEventCommandHandler>>();

        queryMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Event?)null);

        var handler = new UpdateEventCommandHandler(queryMock.Object, commandMock.Object, emailMock.Object, loggerMock.Object);
        var command = CreateCommand(CreateEvent());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_ValidUpdate_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var (handler, _, commandMock, _) = BuildHandler();
        var command = CreateCommand(CreateEvent());

        // Act
        await handler.Handle(command);

        // Assert
        commandMock.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TransitionToPlanned_WithContactEmail_SendsPlannedNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        var (handler, _, _, emailMock) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.Planned, email: "contact@example.com"));

        // Act
        await handler.Handle(command);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TransitionToPlanned_WithoutContactEmail_DoesNotSendPlannedNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: null);
        var (handler, _, _, emailMock) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.Planned, email: null));

        // Act
        await handler.Handle(command);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TransitionToSendInvoice_WithContactEmail_SendsInvoiceNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: "contact@example.com");
        var (handler, _, _, emailMock) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com"));

        // Act
        await handler.Handle(command);

        // Assert
        emailMock.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TransitionToSendInvoice_WithoutContactEmail_DoesNotSendInvoiceNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: null);
        var (handler, _, _, emailMock) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.SendInvoice, email: null));

        // Act
        await handler.Handle(command);

        // Assert
        emailMock.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MismatchedIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var existing = CreateEvent(id: 1);
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(id: 2));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
    }

    [Fact]
    public async Task Handle_UpdatesFields()
    {
        // Arrange
        var existing = CreateEvent();
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent());
        command.Name = "Updated Name";
        command.Location = "New Location";
        command.Description = "New Description";
        command.ContactPerson = "New Person";
        command.ContactPhone = "123456";
        command.ContactEmail = "new@example.com";

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("New Location", result.Location);
        Assert.Equal("New Description", result.Description);
        Assert.Equal("New Person", result.ContactPerson);
        Assert.Equal("123456", result.ContactPhone);
        Assert.Equal("new@example.com", result.ContactEmail);
    }

    [Fact]
    public async Task Handle_SetsUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);
        var existing = CreateEvent();
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent());

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.UpdatedAt >= before);
    }

    [Fact]
    public async Task Handle_TransitionToPlanned_PromotesToConfirmed()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.Planned, email: "contact@example.com"));

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.Equal(EventStatusDTO.Confirmed, result.Status);
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task Handle_TransitionToPlanned_WhenAlreadyNotified_DoesNotSendNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        existing.NotificationSent = true;
        var (handler, _, _, emailMock) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.Planned, email: "contact@example.com"));

        // Act
        await handler.Handle(command);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TransitionToSendInvoice_SetsNotificationSent()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: "contact@example.com");
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com"));

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsUpdatedEvent()
    {
        // Arrange
        var existing = CreateEvent();
        var (handler, _, _, _) = BuildHandler(existing);
        var command = CreateCommand(CreateEvent());
        command.Name = "New Name";

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
    }
}
