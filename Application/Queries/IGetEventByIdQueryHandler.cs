using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetEventByIdQuery"/> and returns a single event by its identifier.
/// </summary>
public interface IGetEventByIdQueryHandler
{
    /// <summary>
    /// Returns the event with the given identifier, or <c>null</c> if not found.
    /// </summary>
    Task<Event?> Handle(GetEventByIdQuery query);
}
