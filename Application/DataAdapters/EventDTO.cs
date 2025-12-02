using System.ComponentModel.DataAnnotations;

namespace Application.DataAdapters;

/// <summary>
/// Represents a medical first aid event
/// </summary>
public class EventDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public EventStatusDTO Status { get; set; } = EventStatusDTO.Requested;

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? ContactEmail { get; set; }

    public bool NotificationSent { get; set; } = false;
        
    // Navigation properties
    public List<ShiftDTO> Shifts { get; set; } = new();
}

/// <summary>
/// Status of an event
/// </summary>
public enum EventStatusDTO
{
    Requested,
    Planned,
    Confirmed,
    Active,
    Completed,
    SendInvoice,
    Cancelled
}
