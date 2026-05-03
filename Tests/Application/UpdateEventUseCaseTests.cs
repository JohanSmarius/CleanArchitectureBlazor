using Application;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="UpdateEventUseCase"/>.
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

    private static (UpdateEventUseCase useCase, Mock<IEventRepository> repoMock, Mock<IEmailService> emailMock)
        BuildUseCase(Event? existingEvent = null)
    {
        var repoMock = new Mock<IEventRepository>();
        var emailMock = new Mock<IEmailService>();
        var loggerMock = new Mock<ILogger<EventService>>();

        repoMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(existingEvent ?? CreateEvent());

        repoMock
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        emailMock
            .Setup(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        emailMock
            .Setup(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        return (new UpdateEventUseCase(repoMock.Object, emailMock.Object, loggerMock.Object), repoMock, emailMock);
    }

    [Fact]
    public async Task Execute_StartDateEqualToEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _, _) = BuildUseCase();
        var updated = CreateEvent();
        updated.EndDate = updated.StartDate;

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_StartDateAfterEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _, _) = BuildUseCase();
        var updated = CreateEvent();
        updated.EndDate = updated.StartDate.AddHours(-1);

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_EventNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var repoMock = new Mock<IEventRepository>();
        var emailMock = new Mock<IEmailService>();
        var loggerMock = new Mock<ILogger<EventService>>();

        repoMock
            .Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Event?)null);

        var useCase = new UpdateEventUseCase(repoMock.Object, emailMock.Object, loggerMock.Object);
        var updated = CreateEvent();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_ValidUpdate_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var (useCase, repoMock, _) = BuildUseCase();
        var updated = CreateEvent();

        // Act
        await useCase.Execute(updated);

        // Assert
        repoMock.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Execute_TransitionToPlanned_WithContactEmail_SendsPlannedNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        var (useCase, _, emailMock) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.Planned, email: "contact@example.com");

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Execute_TransitionToPlanned_WithoutContactEmail_DoesNotSendPlannedNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: null);
        var (useCase, _, emailMock) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.Planned, email: null);

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Execute_TransitionToSendInvoice_WithContactEmail_SendsInvoiceNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: "contact@example.com");
        var (useCase, _, emailMock) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com");

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task Execute_TransitionToSendInvoice_WithoutContactEmail_DoesNotSendInvoiceNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: null);
        var (useCase, _, emailMock) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: null);

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ValidUpdate_ReturnsUpdatedEvent()
    {
        // Arrange
        var existing = CreateEvent();
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent();
        updated.Name = "New Name";

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.NotNull(result);
    }
}
