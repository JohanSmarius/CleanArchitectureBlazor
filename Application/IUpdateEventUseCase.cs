using Entities;
using Microsoft.Extensions.Logging;

namespace Application
{
    public interface IUpdateEventUseCase
    {
        Task<Event> Execute(Event updated);
    }
}