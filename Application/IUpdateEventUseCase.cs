using Entities;
using Application.DataAdapters;
using Microsoft.Extensions.Logging;

namespace Application
{
    public interface IUpdateEventUseCase
    {
        Task<EventDTO> Execute(EventDTO updated);
    }
}