using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetAllEventsQuery"/> and returns all events ordered by start date.
/// </summary>
public interface IGetAllEventsQueryHandler
{
    /// <summary>
    /// Returns all events ordered by start date.
    /// </summary>
    Task<List<Event>> Handle(GetAllEventsQuery query);
}
