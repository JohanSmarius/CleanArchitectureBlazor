using Application.DataAdapters;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="UpdateEventCommand"/> and returns the updated event as a DTO.
/// </summary>
public interface IUpdateEventCommandHandler
{
    /// <summary>
    /// Validates the command, applies changes to the event, and persists it.
    /// </summary>
    /// <param name="command">The data required to update the event.</param>
    /// <returns>A <see cref="EventDTO"/> representing the updated event.</returns>
    Task<EventDTO> Handle(UpdateEventCommand command);
}
