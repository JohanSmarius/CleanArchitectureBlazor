using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
/// Represents assignment of staff to a shift
/// </summary>
public class StaffAssignment
{
    public int Id { get; set; }

    [Required]
    public int ShiftId { get; set; }

    [Required]
    public int StaffId { get; set; }

    public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    [StringLength(300)]
    public string? Notes { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Shift Shift { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
}

/// <summary>
/// Status of a staff assignment
/// </summary>
public enum AssignmentStatus
{
    Assigned,
    Confirmed,
    CheckedIn,
    CheckedOut,
    NoShow,
    Cancelled
}
