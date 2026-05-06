using Entities;
using Application.DataAdapters;

namespace Application;

public interface IEventService
{
    Task<EventDTO> UpdateEventAsync(EventDTO e);
}