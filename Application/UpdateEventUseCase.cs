using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class UpdateEventUseCase : IUpdateEventUseCase
    {
        private readonly IEventRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EventService> _logger;

        public UpdateEventUseCase(
            IEventRepository repository,
            IEmailService emailService,
            ILogger<EventService> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<EventDTO> Execute(EventDTO updated)
        {
            // Validate dates
            if (updated.StartDate >= updated.EndDate)
            {
                throw new ÀpplicationException("End date must be after start date.");
            }

            // Load current state
            var existing = await _repository.GetEventByIdAsync(updated.Id) ??
                throw new InvalidOperationException($"Event {updated.Id} not found");

            if (existing.Id != updated.Id)
                throw new InvalidOperationException("Mismatched Event ids.");

            // Check if date changes affect existing shifts
            if (updated.Shifts.Any() &&
                (updated.StartDate != existing.StartDate || updated.EndDate != existing.EndDate))
            {
                var conflictingShifts = updated.Shifts.Where(s =>
                    s.StartTime < updated.StartDate || s.EndTime > updated.EndDate).ToList();

                if (conflictingShifts.Any())
                {
                    throw new ApplicationException($"Cannot change event dates. {conflictingShifts.Count} shift(s) would fall outside the new event timeframe.");
                }
            }

            // Capture original values needed for rules
            var originalStatus = existing.Status;
            var originalNotificationSent = existing.NotificationSent;

            // Copy over mutable fields (avoid overwriting identity / audit)
            existing.Name = updated.Name;
            existing.StartDate = updated.StartDate;
            existing.EndDate = updated.EndDate;
            existing.Location = updated.Location;
            existing.Description = updated.Description;
            existing.Status = (EventStatus)updated.Status; // may be further changed after notification
            existing.ContactPerson = updated.ContactPerson;
            existing.ContactPhone = updated.ContactPhone;
            existing.ContactEmail = updated.ContactEmail;
            existing.UpdatedAt = DateTime.UtcNow;

            bool canContact = !string.IsNullOrWhiteSpace(existing.ContactEmail);

            bool shouldSendPlanned =
                originalStatus != Entities.EventStatus.Planned &&
                existing.Status == Entities.EventStatus.Planned &&
                !originalNotificationSent &&
                canContact;

            bool shouldSendInvoice =
                originalStatus != Entities.EventStatus.SendInvoice &&
                existing.Status == Entities.EventStatus.SendInvoice &&
                canContact;

            // Perform side-effects (email notifications)
            if (shouldSendPlanned)
            {
                try
                {
                    await _emailService.SendEventPlannedNotificationAsync(existing);
                    // business rule: auto confirm after planned email
                    existing.Status = Entities.EventStatus.Confirmed;
                    existing.NotificationSent = true;
                    _logger.LogInformation("Planned notification sent for Event {EventId}", existing.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed sending planned notification for Event {EventId}", existing.Id);
                }
            }

            if (shouldSendInvoice)
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

            return existing.ToDTO();
        }
    }
}
