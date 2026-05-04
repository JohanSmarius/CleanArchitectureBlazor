using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataAdapters
{
    public static class EventMapper
    {
        public static EventDTO ToDTO(this Entities.Event entityEvent)
        { 
            return new EventDTO
            {
                Id = entityEvent.Id,
                Name = entityEvent.Name,
                StartDate = entityEvent.StartDate,
                EndDate = entityEvent.EndDate,
                Location = entityEvent.Location,
                Description = entityEvent.Description,
                Status = (EventStatusDTO)entityEvent.Status,
                ContactPerson = entityEvent.ContactPerson,
                ContactPhone = entityEvent.ContactPhone,
                ContactEmail = entityEvent.ContactEmail,
                NotificationSent = entityEvent.NotificationSent,
                Shifts = entityEvent.Shifts != null
                    ? entityEvent.Shifts.ToDTOList()
                    : new List<ShiftDTO>()
            };

        }

        public static Entities.Event ToEntity(this EventDTO dtoEvent)
        {
            return new Entities.Event
            {
                Id = dtoEvent.Id,
                Name = dtoEvent.Name,
                StartDate = dtoEvent.StartDate,
                EndDate = dtoEvent.EndDate,
                Location = dtoEvent.Location,
                Description = dtoEvent.Description,
                Status = (Entities.EventStatus)dtoEvent.Status,
                ContactPerson = dtoEvent.ContactPerson,
                ContactPhone = dtoEvent.ContactPhone,
                ContactEmail = dtoEvent.ContactEmail,
                NotificationSent = dtoEvent.NotificationSent,
                Shifts = dtoEvent.Shifts != null
                    ? dtoEvent.Shifts.ToEntityList()
                    : new List<Entities.Shift>()
            };
        }
    }
}
