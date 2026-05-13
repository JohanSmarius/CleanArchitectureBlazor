using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureBlazor.Client.Models;

/// <summary>
/// Represents a staff member
/// </summary>
public class Staff
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [Required]
    public StaffRole Role { get; set; }

    [StringLength(50)]
    public string? CertificationLevel { get; set; }

    public DateTime? CertificationExpiry { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    public List<StaffAssignment> StaffAssignments { get; set; } = new();
}

public class StaffAssignment
{
    public int Id { get; set; }
    public int ShiftId { get; set; }
    public int StaffId { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Notes { get; set; }
    public Shift Shift { get; set; } = null!;
}

public enum AssignmentStatus
{
    Assigned,
    Confirmed,
    CheckedIn,
    CheckedOut,
    NoShow,
    Cancelled
}

public class Shift
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
    public Event Event { get; set; } = null!;
}

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

/// <summary>
/// Role of a staff member
/// </summary>
public enum StaffRole
{
    FirstAider,
    TeamLeader,
    Paramedic,
    Doctor,
    Volunteer
}
