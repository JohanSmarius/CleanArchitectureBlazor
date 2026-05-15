using System.ComponentModel.DataAnnotations;
using Entities;

namespace Application.Commands;

/// <summary>
/// Command data object carrying all information required to update an existing staff member.
/// </summary>
public class UpdateStaffCommand
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

    public DateTime? Birthday { get; set; }

    public bool IsActive { get; set; }
}
