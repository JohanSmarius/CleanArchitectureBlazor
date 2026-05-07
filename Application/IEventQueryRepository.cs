using Entities;

namespace Application;

/// <summary>
/// Read-side repository interface for event retrieval (CQRS query side).
/// </summary>
public interface IEventQueryRepository
{
    /// <summary>Returns all events, ordered by start date.</summary>
    Task<List<Event>> GetAllEventsAsync();

    /// <summary>Returns the event with the given identifier, or <c>null</c> if not found.</summary>
    Task<Event?> GetEventByIdAsync(int id);

    /// <summary>Returns upcoming (future, non-cancelled) events.</summary>
    Task<List<Event>> GetUpcomingEventsAsync();

    /// <summary>Returns events whose start date falls within the specified range.</summary>
    Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
