using System.ComponentModel.DataAnnotations;
using Entities;

namespace Application.DataAdapters;

public class StaffAssignmentDTO
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
}