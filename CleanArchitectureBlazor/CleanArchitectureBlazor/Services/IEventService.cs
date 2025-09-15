using Domain;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing events
/// </summary>
public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task<Event> CreateEventAsync(Event eventModel);
    Task<Event> UpdateEventAsync(Event eventModel);
    Task DeleteEventAsync(int id);
    Task<List<Event>> GetUpcomingEventsAsync();
    Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
