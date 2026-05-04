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
        // Given
        var entity = CreateEventEntity();

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.Equal(5, dto.Id);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenNameIsMapped()
    {
        // Given
        var entity = CreateEventEntity();

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.Equal("Charity Run", dto.Name);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenStatusIsMapped()
    {
        // Given
        var entity = CreateEventEntity();

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.Equal(EventStatusDTO.Confirmed, dto.Status);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenContactEmailIsMapped()
    {
        // Given
        var entity = CreateEventEntity();

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.Equal("alice@example.com", dto.ContactEmail);
    }

    [Fact]
    public void GivenEventEntity_WhenToDTOCalled_ThenNotificationSentIsMapped()
    {
        // Given
        var entity = CreateEventEntity();

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.True(dto.NotificationSent);
    }

    [Fact]
    public void GivenEventEntityWithNullShifts_WhenToDTOCalled_ThenShiftsIsEmpty()
    {
        // Given
        var entity = CreateEventEntity();
        entity.Shifts = null!;

        // When
        var dto = entity.ToDTO();

        // Then
        Assert.Empty(dto.Shifts);
    }

    // ── ToEntity ───────────────────────────────────────────────────────────

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenIdIsMapped()
    {
        // Given
        var dto = CreateEventDTO();

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal(5, entity.Id);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenNameIsMapped()
    {
        // Given
        var dto = CreateEventDTO();

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal("Charity Run", entity.Name);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenStatusIsMapped()
    {
        // Given
        var dto = CreateEventDTO();

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal(EventStatus.Confirmed, entity.Status);
    }

    [Fact]
    public void GivenEventDTO_WhenToEntityCalled_ThenLocationIsMapped()
    {
        // Given
        var dto = CreateEventDTO();

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal("City Park", entity.Location);
    }

    [Fact]
    public void GivenEventDTOWithNullShifts_WhenToEntityCalled_ThenShiftsIsEmpty()
    {
        // Given
        var dto = CreateEventDTO();
        dto.Shifts = null!;

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Empty(entity.Shifts);
    }

    // ── ShiftMapper ────────────────────────────────────────────────────────

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenIdIsMapped()
    {
        // Given
        var shift = new Shift { Id = 3, Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // When
        var dto = shift.ToDTO();

        // Then
        Assert.Equal(3, dto.Id);
    }

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenNameIsMapped()
    {
        // Given
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // When
        var dto = shift.ToDTO();

        // Then
        Assert.Equal("Morning", dto.Name);
    }

    [Fact]
    public void GivenShiftEntity_WhenToDTOCalled_ThenStatusIsMapped()
    {
        // Given
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), Status = ShiftStatus.Full };

        // When
        var dto = shift.ToDTO();

        // Then
        Assert.Equal(ShiftStatusDTO.Full, dto.Status);
    }

    [Fact]
    public void GivenShiftDTO_WhenToEntityCalled_ThenIdIsMapped()
    {
        // Given
        var dto = new ShiftDTO { Id = 7, Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal(7, entity.Id);
    }

    [Fact]
    public void GivenShiftDTO_WhenToEntityCalled_ThenRequiredStaffIsMapped()
    {
        // Given
        var dto = new ShiftDTO { Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), RequiredStaff = 5 };

        // When
        var entity = dto.ToEntity();

        // Then
        Assert.Equal(5, entity.RequiredStaff);
    }
}
