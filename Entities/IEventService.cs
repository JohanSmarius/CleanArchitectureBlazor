namespace Entities;

public interface IEventService
{
    Task<Event> UpdateEventAsync(Event e);
}