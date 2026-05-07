using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetEventsByDateRangeQuery"/> and returns events within a date range.
/// </summary>
public interface IGetEventsByDateRangeQueryHandler
{
    /// <summary>
    /// Returns events whose start date falls within the specified range.
    /// </summary>
    Task<List<Event>> Handle(GetEventsByDateRangeQuery query);
}
