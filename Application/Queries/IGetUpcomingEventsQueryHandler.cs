using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetUpcomingEventsQuery"/> and returns upcoming events.
/// </summary>
public interface IGetUpcomingEventsQueryHandler
{
    /// <summary>
    /// Returns all upcoming (future, non-cancelled) events.
    /// </summary>
    Task<List<Event>> Handle(GetUpcomingEventsQuery query);
}
