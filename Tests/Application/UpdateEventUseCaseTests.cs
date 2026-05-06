using Application;
using Application.DataAdapters;
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
        var updated = CreateEvent().ToDTO();
        updated.EndDate = updated.StartDate;

        // Act & Assert
        await Assert.ThrowsAsync<global::Entities.ÀpplicationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_StartDateAfterEndDate_ThrowsApplicationException()
    {
        // Arrange
        var (useCase, _, _) = BuildUseCase();
        var updated = CreateEvent().ToDTO();
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
        var updated = CreateEvent().ToDTO();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_ValidUpdate_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var (useCase, repoMock, _) = BuildUseCase();
        var updated = CreateEvent().ToDTO();

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
        var updated = CreateEvent(status: EventStatus.Planned, email: "contact@example.com").ToDTO();

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
        var updated = CreateEvent(status: EventStatus.Planned, email: null).ToDTO();

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
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com").ToDTO();

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
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: null).ToDTO();

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Execute_MismatchedIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var existing = CreateEvent(id: 1);
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent(id: 2).ToDTO();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Execute(updated));
    }

    [Fact]
    public async Task Execute_UpdatesFields()
    {
        // Arrange
        var existing = CreateEvent();
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent().ToDTO();
        updated.Name = "Updated Name";
        updated.Location = "New Location";
        updated.Description = "New Description";
        updated.ContactPerson = "New Person";
        updated.ContactPhone = "123456";
        updated.ContactEmail = "new@example.com";

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("New Location", result.Location);
        Assert.Equal("New Description", result.Description);
        Assert.Equal("New Person", result.ContactPerson);
        Assert.Equal("123456", result.ContactPhone);
        Assert.Equal("new@example.com", result.ContactEmail);
    }

    [Fact]
    public async Task Execute_SetsUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);
        var existing = CreateEvent();
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent().ToDTO();

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.True(result.UpdatedAt >= before);
    }

    [Fact]
    public async Task Execute_TransitionToPlanned_PromotesToConfirmed()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.Planned, email: "contact@example.com").ToDTO();

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.Equal(EventStatusDTO.Confirmed, result.Status);
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task Execute_TransitionToPlanned_WhenAlreadyNotified_DoesNotSendNotification()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        existing.NotificationSent = true;
        var (useCase, _, emailMock) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.Planned, email: "contact@example.com").ToDTO();

        // Act
        await useCase.Execute(updated);

        // Assert
        emailMock.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task Execute_TransitionToSendInvoice_SetsNotificationSent()
    {
        // Arrange
        var existing = CreateEvent(status: EventStatus.Completed, email: "contact@example.com");
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com").ToDTO();

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task Execute_ValidUpdate_ReturnsUpdatedEvent()
    {
        // Arrange
        var existing = CreateEvent();
        var (useCase, _, _) = BuildUseCase(existing);
        var updated = CreateEvent().ToDTO();
        updated.Name = "New Name";

        // Act
        var result = await useCase.Execute(updated);

        // Assert
        Assert.NotNull(result);
    }
}
