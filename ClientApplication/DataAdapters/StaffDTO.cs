using System.ComponentModel.DataAnnotations;

namespace ClientApplication.DataAdapters;

public class StaffDTO
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
    public StaffRoleDTO Role { get; set; }
    
    public bool IsActive { get; set; } = true;

    [StringLength(50)]
    public string? CertificationLevel { get; set; }

    public DateTime? CertificationExpiry { get; set; }

    public DateTime? Birthday { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public List<StaffAssignmentDTO> StaffAssignments { get; set; } = new();
    
    
}

public enum StaffRoleDTO
{
    FirstAider,
    TeamLeader,
    Paramedic,
    Doctor,
    Volunteer
}