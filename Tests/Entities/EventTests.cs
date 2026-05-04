using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Event"/> entity.
/// </summary>
public class EventTests
{
    [Fact]
    public void GivenNewEvent_WhenCreated_ThenStatusIsRequested()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(EventStatus.Requested, @event.Status);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenNotificationSentIsFalse()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.False(@event.NotificationSent);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenShiftsIsEmpty()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Empty(@event.Shifts);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenNameIsEmpty()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(string.Empty, @event.Name);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenLocationIsEmpty()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(string.Empty, @event.Location);
    }

    [Fact]
    public void GivenNewEvent_WhenNameIsSet_ThenNameIsUpdated()
    {
        // Arrange
        var @event = new Event();

        // Act
        @event.Name = "Marathon Medical Cover";

        // Assert
        Assert.Equal("Marathon Medical Cover", @event.Name);
    }

    [Fact]
    public void GivenNewEvent_WhenShiftAdded_ThenShiftsContainsOneShift()
    {
        // Arrange
        var @event = new Event();
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(4) };

        // Act
        @event.Shifts.Add(shift);

        // Assert
        Assert.Single(@event.Shifts);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenCreatedAtIsSet()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var @event = new Event();

        // Assert
        Assert.True(@event.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Null(@event.UpdatedAt);
    }
}
