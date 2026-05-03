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
    public void ToDTO_MapsId()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal(5, dto.Id);
    }

    [Fact]
    public void ToDTO_MapsName()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal("Charity Run", dto.Name);
    }

    [Fact]
    public void ToDTO_MapsStatus()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal(EventStatusDTO.Confirmed, dto.Status);
    }

    [Fact]
    public void ToDTO_MapsContactEmail()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.Equal("alice@example.com", dto.ContactEmail);
    }

    [Fact]
    public void ToDTO_MapsNotificationSent()
    {
        // Arrange
        var entity = CreateEventEntity();

        // Act
        var dto = entity.ToDTO();

        // Assert
        Assert.True(dto.NotificationSent);
    }

    [Fact]
    public void ToDTO_NullShifts_ReturnsEmptyShiftsList()
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
    public void ToEntity_MapsId()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(5, entity.Id);
    }

    [Fact]
    public void ToEntity_MapsName()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal("Charity Run", entity.Name);
    }

    [Fact]
    public void ToEntity_MapsStatus()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(EventStatus.Confirmed, entity.Status);
    }

    [Fact]
    public void ToEntity_MapsLocation()
    {
        // Arrange
        var dto = CreateEventDTO();

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal("City Park", entity.Location);
    }

    [Fact]
    public void ToEntity_NullShifts_ReturnsEmptyShiftsList()
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
    public void ShiftToDTO_MapsId()
    {
        // Arrange
        var shift = new Shift { Id = 3, Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal(3, dto.Id);
    }

    [Fact]
    public void ShiftToDTO_MapsName()
    {
        // Arrange
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal("Morning", dto.Name);
    }

    [Fact]
    public void ShiftToDTO_MapsStatus()
    {
        // Arrange
        var shift = new Shift { Name = "Morning", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), Status = ShiftStatus.Full };

        // Act
        var dto = shift.ToDTO();

        // Assert
        Assert.Equal(ShiftStatusDTO.Full, dto.Status);
    }

    [Fact]
    public void ShiftToEntity_MapsId()
    {
        // Arrange
        var dto = new ShiftDTO { Id = 7, Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4) };

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(7, entity.Id);
    }

    [Fact]
    public void ShiftToEntity_MapsRequiredStaff()
    {
        // Arrange
        var dto = new ShiftDTO { Name = "Evening", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(4), RequiredStaff = 5 };

        // Act
        var entity = dto.ToEntity();

        // Assert
        Assert.Equal(5, entity.RequiredStaff);
    }
}
