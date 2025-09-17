using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class UpdateEventUseCase
    {
        private readonly IEventRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EventService> _logger;
        private readonly EventDomainService _domainService = new();

        public UpdateEventUseCase(
            IEventRepository repository,
            IEmailService emailService,
            ILogger<EventService> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Event> UpdateEventAsync(Event updated)
        {
            // Validate dates
            if (updated.StartDate >= updated.EndDate)
            {
                throw new ÀpplicationException("End date must be after start date.");
            }

            // Check if date changes affect existing shifts
            if (updated.Shifts.Any() &&
                (updated.StartDate != updated.StartDate || updated.EndDate != updated.EndDate))
            {
                var conflictingShifts = updated.Shifts.Where(s =>
                    s.StartTime < updated.StartDate || s.EndTime > updated.EndDate).ToList();

                if (conflictingShifts.Any())
                {
                    throw new ÀpplicationException($"Cannot change event dates. {conflictingShifts.Count} shift(s) would fall outside the new event timeframe.");
                }
            }

            // Load current state
            var existing = await _repository.GetEventByIdAsync(updated.Id) ??
                throw new InvalidOperationException($"Event {updated.Id} not found");

            // Apply domain logic
            var decision = _domainService.ApplyChanges(existing, updated);

            // Perform side-effects (email notifications) based on decision
            if (decision.ShouldSendPlannedNotification)
            {
                try
                {
                    await _emailService.SendEventPlannedNotificationAsync(existing);
                    if (decision.PromoteToConfirmedAfterPlanned)
                    {
                        existing.Status = Entities.EventStatus.Confirmed;
                        existing.NotificationSent = true;
                    }
                    _logger.LogInformation("Planned notification sent for Event {EventId}", existing.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed sending planned notification for Event {EventId}", existing.Id);
                }
            }

            if (decision.ShouldSendInvoiceNotification)
            {
                try
                {
                    await _emailService.SendEventInvoiceNotificationAsync(existing);
                    existing.NotificationSent = true;
                    _logger.LogInformation("Invoice notification sent for Event {EventId}", existing.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed sending invoice notification for Event {EventId}", existing.Id);
                }
            }

            // Persist final state
            await _repository.UpdateEventAsync(existing);

            return existing;
        }
    }
}
