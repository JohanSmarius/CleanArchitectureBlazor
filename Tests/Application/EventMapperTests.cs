using Application.DataAdapters;
using Entities;

namespace Tests.Application;

/// <summary>
/// Unit tests for <see cref="EventMapper"/> and <see cref="ShiftMapper"/>.
/// </summary>
public class EventMapperTests
{
    private static Event CreateEventEntity() => new()
    {
        Id = 5,
        Name = "Charity Run",
        StartDate = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc),
        EndDate = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
        Location = "City Park",
        Description = "Annual charity run",
        Status = EventStatus.Confirmed,
        ContactPerson = "Alice",
        ContactPhone = "0123456789",
        ContactEmail = "alice@example.com",
        NotificationSent = true,
        Shifts = new List<Shift>()
    };

    private static EventDTO CreateEventDTO() => new()
    {
        Id = 5,
        Name = "Charity Run",
        StartDate = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc),
        EndDate = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
        Location = "City Park",
        Description = "Annual charity run",
        Status = EventStatusDTO.Confirmed,
        ContactPerson = "Alice",
        ContactPhone = "0123456789",
        ContactEmail = "alice@example.com",
        NotificationSent = true,
        Shifts = new List<ShiftDTO>()
    };

    // ── ToDTO ──────────────────────────────────────────────────────────────

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenIdIsMapped()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal(5, dto.Id);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenNameIsMapped()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal("Charity Run", dto.Name);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenStatusIsMapped()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal(EventStatusDTO.Confirmed, dto.Status);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenContactEmailIsMapped()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal("alice@example.com", dto.ContactEmail);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenNotificationSentIsMapped()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.True(dto.NotificationSent);
    }

    [Fact]
    public void GivenEventEntityWithNullShifts_WhenToDTOCalled_ThenShiftsIsEmpty()
    {
        // Arrange
        var entity = CreateEventEntity();
        entity.Shifts = null!;

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Empty(dto.Shifts);
    }

    // ── ToEntity ───────────────────────────────────────────────────────────

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenIdIsMapped()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(5, entity.Id);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenNameIsMapped()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal("Charity Run", entity.Name);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenStatusIsMapped()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(EventStatus.Confirmed, entity.Status);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenLocationIsMapped()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal("City Park", entity.Location);
    }

    [Fact]
    public void GivenEventDTOWithNullShifts_WhenToEntityCalled_ThenShiftsIsEmpty()
    {
        // Arrange
        var dto = CreateEventDTO();
        dto.Shifts = null!;

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Empty(entity.Shifts);
    }

    // ── ShiftMapper ────────────────────────────────────────────────────────

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenIdIsMapped()
    {
        // Arrange
        var shift = new Shift { Id = 3, Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal(3, dto.Id);
    }

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenNameIsMapped()
    {
        // Arrange
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal("Morning", dto.Name);
    }

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenStatusIsMapped()
    {
        // Arrange
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), Status = ShiftStatus.Full };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal(ShiftStatusDTO.Full, dto.Status);
    }

    [Fact]
    public void GivenShiftDTO_WhenToEntityCalled_ThenIdIsMapped()
    {
        // Arrange
        var dto = new ShiftDTO { Id = 7, Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(7, entity.Id);
    }

    [Fact]
    public void GivenShiftDTO_WhenToEntityCalled_ThenRequiredStaffIsMapped()
    {
        // Arrange
        var dto = new ShiftDTO { Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), RequiredStaff = 5 };

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(5, entity.RequiredStaff);
    }
}
