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
        // Given & When
        var @event = new Event();

        // Then
        Assert.Equal(EventStatus.Requested, @event.Status);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenNotificationSentIsFalse()
    {
        // Given & When
        var @event = new Event();

        // Then
        Assert.False(@event.NotificationSent);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenShiftsIsEmpty()
    {
        // Given & When
        var @event = new Event();

        // Then
        Assert.Empty(@event.Shifts);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenNameIsEmpty()
    {
        // Given & When
        var @event = new Event();

        // Then
        Assert.Equal(string.Empty, @event.Name);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenLocationIsEmpty()
    {
        // Given & When
        var @event = new Event();

        // Then
        Assert.Equal(string.Empty, @event.Location);
    }

    [Fact]
    public void GivenNewEvent_WhenNameIsSet_ThenNameIsUpdated()
    {
        // Given
        var @event = new Event();

        // When
        @event.Name = "Marathon Medical Cover";

        // Then
        Assert.Equal("Marathon Medical Cover", @event.Name);
    }

    [Fact]
    public void GivenNewEvent_WhenShiftAdded_ThenShiftsContainsOneShift()
    {
        // Given
        var @event = new Event();
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(4) };

        // When
        @event.Shifts.Add(shift);

        // Then
        Assert.Single(@event.Shifts);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenCreatedAtIsSet()
    {
        // Given
        var before = DateTime.UtcNow.AddSeconds(-1);

        // When
        var @event = new Event();

        // Then
        Assert.True(@event.CreatedAt >= before);
    }

    [Fact]
    public void GivenNewEvent_WhenCreated_ThenUpdatedAtIsNull()
    {
        // Given & When
        var @event = new Event();

        // Then
        Assert.Null(@event.UpdatedAt);
    }
}
