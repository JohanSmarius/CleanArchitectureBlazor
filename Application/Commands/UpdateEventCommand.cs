using Application.DataAdapters;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands;

/// <summary>
/// Command data object carrying all information required to update an existing event.
/// </summary>
public class UpdateEventCommand
{
    /// <summary>Identifier of the event to update.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the event.</summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Date and time when the event starts.</summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>Date and time when the event ends (must be after <see cref="StartDate"/>).</summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>Physical or virtual location of the event.</summary>
    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    /// <summary>Optional description providing additional details about the event.</summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>Target status for the event.</summary>
    public EventStatusDTO Status { get; set; }

    /// <summary>Optional name of the primary contact person for the event.</summary>
    [StringLength(100)]
    public string? ContactPerson { get; set; }

    /// <summary>Optional phone number of the contact person.</summary>
    [StringLength(20)]
    public string? ContactPhone { get; set; }

    /// <summary>Optional e-mail address of the contact person.</summary>
    [EmailAddress]
    [StringLength(255)]
    public string? ContactEmail { get; set; }

    /// <summary>Current shifts on the event (used for date-conflict validation).</summary>
    public List<ShiftDTO> Shifts { get; set; } = new();
}
