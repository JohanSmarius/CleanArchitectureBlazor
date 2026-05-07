using Entities;

namespace Application;

/// <summary>
/// Write-side repository interface for event persistence (CQRS command side).
/// </summary>
public interface IEventCommandRepository
{
    /// <summary>Creates and persists a new event.</summary>
    Task<Event> CreateEventAsync(Event eventModel);

    /// <summary>Updates an existing event.</summary>
    Task<Event> UpdateEventAsync(Event eventModel);

    /// <summary>Deletes the event with the specified identifier.</summary>
    Task DeleteEventAsync(int id);
}
