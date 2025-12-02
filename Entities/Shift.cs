using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Represents a shift within an event
/// </summary>
public class Shift
{
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, 50)]
    public int RequiredStaff { get; set; } = 1;

    [StringLength(300)]
    public string? Description { get; set; }

    public ShiftStatus Status { get; set; } = ShiftStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Event Event { get; set; } = null!;
    public List<StaffAssignment> StaffAssignments { get; set; } = new();
}

/// <summary>
/// Status of a shift
/// </summary>
public enum ShiftStatus
{
    Open,
    Full,
    InProgress,
    Completed,
    Cancelled
}
