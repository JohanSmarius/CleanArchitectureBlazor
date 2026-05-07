using Application.Commands;

namespace Application.DataAdapters;

/// <summary>
/// Extension methods that map an <see cref="EventDTO"/> to CQRS command objects.
/// </summary>
public static class EventCommandMapper
{
    /// <summary>
    /// Creates a <see cref="CreateEventCommand"/> from the relevant fields of this DTO.
    /// </summary>
    public static CreateEventCommand ToCreateCommand(this EventDTO dto) => new()
    {
        Name = dto.Name,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Location = dto.Location,
        Description = dto.Description,
        ContactPerson = dto.ContactPerson,
        ContactPhone = dto.ContactPhone,
        ContactEmail = dto.ContactEmail,
    };

    /// <summary>
    /// Creates an <see cref="UpdateEventCommand"/> from all mutable fields of this DTO.
    /// </summary>
    public static UpdateEventCommand ToUpdateCommand(this EventDTO dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Location = dto.Location,
        Description = dto.Description,
        Status = dto.Status,
        ContactPerson = dto.ContactPerson,
        ContactPhone = dto.ContactPhone,
        ContactEmail = dto.ContactEmail,
        Shifts = dto.Shifts,
    };
}
