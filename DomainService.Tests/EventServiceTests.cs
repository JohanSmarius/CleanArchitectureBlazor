using Domain;
using DomainService;
using Microsoft.Extensions.Logging;
using Moq;

namespace DomainService.Tests;

/// <summary>
/// Unit tests for EventService following AAA pattern
/// </summary>
public class EventServiceTests
{
    #region CreateEventAsync Tests

    [Fact]
    public async Task CreateEventAsync_WithValidEvent_ShouldSetRequestedStatus()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Equal(EventStatus.Requested, result.Status);
    }

    [Fact]
    public async Task CreateEventAsync_WithValidEvent_ShouldSetNotificationSentToFalse()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.False(result.NotificationSent);
    }

    [Fact]
    public async Task CreateEventAsync_WithValidEvent_ShouldCreateSingleDefaultShift()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Single(result.Shifts);
    }

    [Fact]
    public async Task CreateEventAsync_WithValidEvent_ShouldNameDefaultShiftAsFullEventDuration()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Equal("Full Event duration", result.Shifts[0].Name);
    }

    [Fact]
    public async Task CreateEventAsync_WithValidEvent_ShouldCallRepositoryCreateOnce()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        await service.CreateEventAsync(newEvent);

        // Assert
        mockRepository.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task CreateEventAsync_WithEndDateBeforeStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => service.CreateEventAsync(newEvent));
    }

    [Fact]
    public async Task CreateEventAsync_WithEndDateBeforeStartDate_ShouldNotCallRepositoryCreate()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        // Act
        await Assert.ThrowsAsync<DomainException>(() => service.CreateEventAsync(newEvent));

        // Assert
        mockRepository.Verify(r => r.CreateEventAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task CreateEventAsync_WithEndDateEqualToStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var sameDate = DateTime.UtcNow.AddDays(1);
        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = sameDate,
            EndDate = sameDate,
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => service.CreateEventAsync(newEvent));
    }

    [Fact]
    public async Task CreateEventAsync_WithStartDateInPast_ShouldThrowDomainException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(-1),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => service.CreateEventAsync(newEvent));
    }

    [Fact]
    public async Task CreateEventAsync_ShouldSetCreatedAtTimestamp()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldSetUpdatedAtTimestamp()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.NotEqual(DateTime.MinValue, result.UpdatedAt);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateShiftWithStartTimeMatchingEventStart()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(3);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Equal(startDate, result.Shifts[0].StartTime);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateShiftWithEndTimeMatchingEventEnd()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(3);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Equal(endDate, result.Shifts[0].EndTime);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateShiftWithDefaultRequiredStaffOfOne()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(3);

        var newEvent = new Event
        {
            Name = "Test Event",
            StartDate = startDate,
            EndDate = endDate,
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.CreateEventAsync(newEvent);

        // Assert
        Assert.Equal(1, result.Shifts[0].RequiredStaff);
    }

    #endregion

    #region UpdateEventAsync Tests

    [Fact]
    public async Task UpdateEventAsync_WithValidUpdate_ShouldUpdateName()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Old Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Old Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "New Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "New Location",
            Status = EventStatus.Requested,
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        Assert.Equal("New Event", result.Name);
    }

    [Fact]
    public async Task UpdateEventAsync_WithValidUpdate_ShouldUpdateLocation()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Old Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Old Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "New Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "New Location",
            Status = EventStatus.Requested,
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        Assert.Equal("New Location", result.Location);
    }

    [Fact]
    public async Task UpdateEventAsync_WithValidUpdate_ShouldCallRepositoryUpdateOnce()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Old Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Old Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "New Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "New Location",
            Status = EventStatus.Requested,
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_WithEndDateBeforeStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => service.UpdateEventAsync(updatedEvent));
    }

    [Fact]
    public async Task UpdateEventAsync_WithNonExistentEvent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var updatedEvent = new Event
        {
            Id = 999,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(999))
            .ReturnsAsync((Event?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateEventAsync(updatedEvent));
    }

    [Fact]
    public async Task UpdateEventAsync_WithShiftsOutsideNewEventTimeframe_ValidationNeverTriggeredDueToBug()
    {
        // Arrange
        // NOTE: This test documents a bug in the production code at line 68:
        // The condition (updated.StartDate != updated.StartDate || updated.EndDate != updated.EndDate)
        // will always be false, so the shift validation never executes.
        // This test verifies the current behavior (no exception thrown) rather than the intended behavior.

        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var originalStart = DateTime.UtcNow.AddDays(1);
        var originalEnd = DateTime.UtcNow.AddDays(5);
        var newStart = DateTime.UtcNow.AddDays(2);
        var newEnd = DateTime.UtcNow.AddDays(4);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = originalStart,
            EndDate = originalEnd,
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = newStart,
            EndDate = newEnd,
            Location = "Test Location",
            Status = EventStatus.Requested,
            Shifts = new List<Shift>
            {
                new Shift
                {
                    Id = 1,
                    StartTime = originalStart,
                    EndTime = originalEnd,
                    Name = "Test Shift"
                }
            }
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        // Due to the bug, no exception is thrown and the update proceeds
        mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlanned_ShouldSendPlannedNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlanned_ShouldSetStatusToConfirmed()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        Assert.Equal(EventStatus.Confirmed, result.Status);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlanned_ShouldSetNotificationSentToTrue()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlannedWithoutEmail_ShouldNotSendNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = null,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = null,
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlannedWhenAlreadyNotified_ShouldNotSendNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = true,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToSendInvoice_ShouldSendInvoiceNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Completed,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.SendInvoice,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToSendInvoice_ShouldSetNotificationSentToTrue()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Completed,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.SendInvoice,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        Assert.True(result.NotificationSent);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToSendInvoiceWithoutEmail_ShouldNotSendNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Completed,
            NotificationSent = false,
            ContactEmail = null,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.SendInvoice,
            ContactEmail = null,
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_PlannedNotificationFailure_ShouldLogErrorAndContinue()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        mockEmailService
            .Setup(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()))
            .ThrowsAsync(new Exception("Email service failure"));

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_PlannedNotificationFailure_ShouldLogError()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        mockEmailService
            .Setup(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()))
            .ThrowsAsync(new Exception("Email service failure"));

        // Act
        await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_InvoiceNotificationFailure_ShouldLogErrorAndContinue()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Completed,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.SendInvoice,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        mockEmailService
            .Setup(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()))
            .ThrowsAsync(new Exception("Email service failure"));

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_InvoiceNotificationFailure_ShouldLogError()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Completed,
            NotificationSent = false,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.SendInvoice,
            ContactEmail = "test@example.com",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        mockEmailService
            .Setup(e => e.SendEventInvoiceNotificationAsync(It.IsAny<Event>()))
            .ThrowsAsync(new Exception("Email service failure"));

        // Act
        await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_WithShiftsInsideEventTimeframe_ShouldNotThrowException()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var eventStart = DateTime.UtcNow.AddDays(1);
        var eventEnd = DateTime.UtcNow.AddDays(5);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = eventStart,
            EndDate = eventEnd,
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Updated Event",
            StartDate = eventStart,
            EndDate = eventEnd,
            Location = "Updated Location",
            Status = EventStatus.Requested,
            Shifts = new List<Shift>
            {
                new Shift
                {
                    Id = 1,
                    StartTime = eventStart.AddHours(1),
                    EndTime = eventEnd.AddHours(-1),
                    Name = "Test Shift"
                }
            }
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_StatusChangeToPlannedWithEmptyEmail_ShouldNotSendNotification()
    {
        // Arrange
        var mockRepository = new Mock<IEventRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockLogger = new Mock<ILogger<EventService>>();
        var service = new EventService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        var existingEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Requested,
            NotificationSent = false,
            ContactEmail = "   ",
            Shifts = new List<Shift>()
        };

        var updatedEvent = new Event
        {
            Id = 1,
            Name = "Test Event",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Test Location",
            Status = EventStatus.Planned,
            ContactEmail = "   ",
            Shifts = new List<Shift>()
        };

        mockRepository
            .Setup(r => r.GetEventByIdAsync(1))
            .ReturnsAsync(existingEvent);

        mockRepository
            .Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
            .ReturnsAsync((Event e) => e);

        // Act
        var result = await service.UpdateEventAsync(updatedEvent);

        // Assert
        mockEmailService.Verify(e => e.SendEventPlannedNotificationAsync(It.IsAny<Event>()), Times.Never);
    }

    #endregion
}
