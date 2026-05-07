using Application.DataAdapters;

namespace Application;

/// <summary>
/// Use-case abstraction for updating an existing event.
/// </summary>
public interface IUpdateEventUseCase
{
    /// <summary>
    /// Executes the update flow for the supplied event DTO.
    /// </summary>
    Task<EventDTO> Execute(EventDTO updated);
}
