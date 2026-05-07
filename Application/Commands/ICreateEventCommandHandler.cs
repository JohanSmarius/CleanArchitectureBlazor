using Application.DataAdapters;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="CreateEventCommand"/> and returns the persisted event as a DTO.
/// </summary>
public interface ICreateEventCommandHandler
{
    /// <summary>
    /// Validates the command, creates the event entity, and persists it.
    /// </summary>
    /// <param name="command">The data required to create the event.</param>
    /// <returns>A <see cref="EventDTO"/> representing the newly created event.</returns>
    Task<EventDTO> Handle(CreateEventCommand command);
}
