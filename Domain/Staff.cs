using System.ComponentModel.DataAnnotations;

namespace Domain;

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

    public DateTime? Birthday 
    { 
			get => field;
            set
            {
                if (value == null)
                {
                    field = null;
                    return;
                };
                
                var today = DateTime.Today;
                var age = today.Year - value.Value.Year;
                if (value.Value.Date > today.AddYears(-age)) age--;
                if (age < 18)
                {
                    throw new ArgumentException("Staff must be at least 18 years old");
                }
                field = value;
			}
    }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    public List<StaffAssignment> StaffAssignments { get; set; } = new();
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
