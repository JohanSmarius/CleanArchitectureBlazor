using Application.DataAdapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface ICreateEventUseCase
    {
        Task<EventDTO> CreateEventAsync(EventDTO newEvent);
    }
}
