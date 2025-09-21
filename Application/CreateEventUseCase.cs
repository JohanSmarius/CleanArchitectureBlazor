using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class CreateEventUseCase : ICreateEventUseCase
    {
        private readonly IEventRepository repository;
        private readonly ILogger<EventService> logger;

        public CreateEventUseCase(
            IEventRepository repository,
            ILogger<EventService> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<EventDTO> Execute(EventDTO newEvent)
        {
            // Validate dates
            if (newEvent.StartDate >= newEvent.EndDate)
            {
                throw new ÀpplicationException("End date must be after start date.");
            }

            if (newEvent.StartDate <= DateTime.UtcNow)
            {
                throw new ÀpplicationException("Start date must be in the future.");
            }

            var entity = newEvent.ToEntity();
            entity.Status = EventStatus.Requested;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            // Create a shift to cover the entire event duration by default
            entity.Shifts.Add(new Shift
            {
                Name = "Default Shift",
                StartTime = newEvent.StartDate,
                EndTime = newEvent.EndDate,
                RequiredStaff = 1,
                Description = "Default shift covering the entire event duration",
                Status = ShiftStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            var createdEvent = await repository.CreateEventAsync(entity);

            logger.LogInformation($"Event {createdEvent.Id} created successfully.");

            return createdEvent.ToDTO();
        }
    }
}
