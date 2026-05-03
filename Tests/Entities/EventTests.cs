using Entities;

namespace Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Event"/> entity.
/// </summary>
public class EventTests
{
    [Fact]
    public void Event_DefaultStatus_IsRequested()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(EventStatus.Requested, @event.Status);
    }

    [Fact]
    public void Event_DefaultNotificationSent_IsFalse()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.False(@event.NotificationSent);
    }

    [Fact]
    public void Event_DefaultShifts_IsEmptyList()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Empty(@event.Shifts);
    }

    [Fact]
    public void Event_DefaultName_IsEmptyString()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(string.Empty, @event.Name);
    }

    [Fact]
    public void Event_DefaultLocation_IsEmptyString()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Equal(string.Empty, @event.Location);
    }

    [Fact]
    public void Event_CanSetAndGetName()
    {
        // Arrange
        var @event = new Event();

        // Act
        @event.Name = "Marathon Medical Cover";

        // Assert
        Assert.Equal("Marathon Medical Cover", @event.Name);
    }

    [Fact]
    public void Event_CanAddShiftToShifts()
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
    public void Event_CreatedAt_IsSetOnConstruction()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var @event = new Event();

        // Assert
        Assert.True(@event.CreatedAt >= before);
    }

    [Fact]
    public void Event_UpdatedAt_DefaultIsNull()
    {
        // Arrange & Act
        var @event = new Event();

        // Assert
        Assert.Null(@event.UpdatedAt);
    }
}
