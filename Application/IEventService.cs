using Entities;

namespace Application;

public interface IEventService
{
    Task<Event> UpdateEventAsync(Event e);
}