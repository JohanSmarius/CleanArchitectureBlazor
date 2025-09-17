namespace Domain;

public interface IEventService
{
    Task<Event> CreateEventAsync(Event newEvent);
    Task<Event> UpdateEventAsync(Event e);
}