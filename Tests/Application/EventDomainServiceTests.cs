using Application;
using Application.DataAdapters;
using Entities;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="EventDomainService.ApplyChanges"/>.
/// </summary>
public class EventDomainServiceTests
{
    private static Event CreateEvent(int id = 1, EventStatus status = EventStatus.Requested, bool notificationSent = false, string? email = "contact@example.com")
        => new()
        {
            Id = id,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = status,
            NotificationSent = notificationSent,
            ContactEmail = email,
        };

    [Fact]
    public void ApplyChanges_MismatchedIds_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(id: 1);
        var updated = CreateEvent(id: 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.ApplyChanges(existing, updated));
    }

    [Fact]
    public void ApplyChanges_UpdatesEventName()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent();
        var updated = CreateEvent();
        updated.Name = "Updated Event Name";

        // Act
        service.ApplyChanges(existing, updated);

        // Assert
        Assert.Equal("Updated Event Name", existing.Name);
    }

    [Fact]
    public void ApplyChanges_UpdatesLocation()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent();
        var updated = CreateEvent();
        updated.Location = "New Venue";

        // Act
        service.ApplyChanges(existing, updated);

        // Assert
        Assert.Equal("New Venue", existing.Location);
    }

    [Fact]
    public void ApplyChanges_SetsUpdatedAt()
    {
        // Arrange
        var service = new EventDomainService();
        var before = DateTime.UtcNow.AddSeconds(-1);
        var existing = CreateEvent();
        var updated = CreateEvent();

        // Act
        service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(existing.UpdatedAt >= before);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithEmail_ShouldSendPlannedNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Requested, notificationSent: false, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.Planned);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(decision.ShouldSendPlannedNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithEmail_ShouldPromoteToConfirmed()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Requested, notificationSent: false, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.Planned);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(decision.PromoteToConfirmedAfterPlanned);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WithoutEmail_ShouldNotSendPlannedNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Requested, notificationSent: false, email: null);
        var updated = CreateEvent(status: EventStatus.Planned, email: null);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToPlanned_WhenAlreadyNotified_ShouldNotSendPlannedNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Requested, notificationSent: true, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.Planned);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
    }

    [Fact]
    public void ApplyChanges_AlreadyPlanned_TransitionToPlanned_ShouldNotSendPlannedNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Planned, notificationSent: false, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.Planned);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendPlannedNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToSendInvoice_WithEmail_ShouldSendInvoiceNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Completed, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.SendInvoice);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.True(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_TransitionToSendInvoice_WithoutEmail_ShouldNotSendInvoiceNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Completed, email: null);
        var updated = CreateEvent(status: EventStatus.SendInvoice, email: null);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_AlreadySendInvoice_TransitionToSendInvoice_ShouldNotSendInvoiceNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.SendInvoice, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.SendInvoice);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendInvoiceNotification);
    }

    [Fact]
    public void ApplyChanges_PlannedTransition_DoesNotSetInvoiceNotification()
    {
        // Arrange
        var service = new EventDomainService();
        var existing = CreateEvent(status: EventStatus.Requested, email: "contact@example.com");
        var updated = CreateEvent(status: EventStatus.Planned);

        // Act
        var decision = service.ApplyChanges(existing, updated);

        // Assert
        Assert.False(decision.ShouldSendInvoiceNotification);
    }
}
