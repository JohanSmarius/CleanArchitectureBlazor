using Application.DataAdapters;

namespace Application;

/// <summary>
/// Use-case abstraction for creating a new event.
/// </summary>
public interface ICreateEventUseCase
{
    /// <summary>
    /// Executes the create-event flow for the supplied event DTO.
    /// </summary>
    Task<EventDTO> Execute(EventDTO newEvent);
}
