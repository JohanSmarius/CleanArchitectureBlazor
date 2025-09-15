namespace Domain;

public interface IEventService
{
    Task<Event> UpdateEventAsync(Event e);
}