using DomainService;
using Microsoft.Extensions.Logging;

namespace DomainService;

public class EventService : IEventService
{
    private readonly IEventRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<EventService> _logger;
        
    public EventService(
        IEventRepository repository,
        IEmailService emailService,
        ILogger<EventService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Event> CreateEventAsync(Event newEvent)
    {
        ValidateEventDates(newEvent.StartDate, newEvent.EndDate);

 

        newEvent.Status = EventStatus.Requested;
        newEvent.NotificationSent = false;
        newEvent.CreatedAt = DateTime.UtcNow;
        newEvent.UpdatedAt = DateTime.UtcNow;

        // add a shift covering the entire event duration if none exist
        newEvent.Shifts.Add(new Shift
        {
            Name = "Full Event duration",
            StartTime = newEvent.StartDate,
            EndTime = newEvent.EndDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Description = "Auto-generated shift covering the entire event duration",
            RequiredStaff = 1
        });

        var created = await _repository.CreateEventAsync(newEvent);

        return created;
    }

    public async Task<Event> UpdateEventAsync(Event updated)
    {
        ValidateEventDates(updated.StartDate, updated.EndDate);

        // Load current state
        var existing = await _repository.GetEventByIdAsync(updated.Id) ??
            throw new InvalidOperationException($"Event {updated.Id} not found");

        // Check if date changes affect existing shifts
        ValidateShiftConstraints(existing, updated);

        // Capture original state needed for rules
        var originalStatus = existing.Status;
        var originalNotificationSent = existing.NotificationSent;

        // Apply field changes
        ApplyFieldChanges(existing, updated);

        // Handle state machine transitions and side-effects
        await HandleStatusTransitionsAsync(existing, originalStatus, originalNotificationSent);

        // Persist final state
        await _repository.UpdateEventAsync(existing);
        return existing;
    }

    private void ValidateEventDates(DateTime startDate, DateTime endDate)
    {
        if (startDate < DateTime.UtcNow)
        {
           throw new DomainException("Start date cannot be in the past.");
        }
        
            
        if (startDate >= endDate)
        {
            throw new DomainException("End date must be after start date.");
        }
    }

    private void ValidateShiftConstraints(Event existing, Event updated)
    {
        if (existing.Shifts.Any() &&
            (updated.StartDate != existing.StartDate || updated.EndDate != existing.EndDate))
        {
            var conflictingShifts = existing.Shifts.Where(s =>
                s.StartTime < updated.StartDate || s.EndTime > updated.EndDate).ToList();

            if (conflictingShifts.Any())
            {
                throw new DomainException($"Cannot change event dates. {conflictingShifts.Count} shift(s) would fall outside the new event timeframe.");
            }
        }
    }

    private void ApplyFieldChanges(Event existing, Event updated)
    {
        existing.Name = updated.Name;
        existing.StartDate = updated.StartDate;
        existing.EndDate = updated.EndDate;
        existing.Location = updated.Location;
        existing.Description = updated.Description;
        existing.Status = updated.Status; // may be further changed after notification
        existing.ContactPerson = updated.ContactPerson;
        existing.ContactPhone = updated.ContactPhone;
        existing.ContactEmail = updated.ContactEmail;
        existing.UpdatedAt = DateTime.UtcNow;
    }

    private async Task HandleStatusTransitionsAsync(Event existing, EventStatus originalStatus, bool originalNotificationSent)
    {
        bool canContact = !string.IsNullOrWhiteSpace(existing.ContactEmail);

        // Transition: Any -> Planned
        if (originalStatus != EventStatus.Planned && existing.Status == EventStatus.Planned && !originalNotificationSent && canContact)
        {
            await HandlePlannedNotificationAsync(existing);
        }
        
        // Transition: Any -> SendInvoice
        if (originalStatus != EventStatus.SendInvoice && existing.Status == EventStatus.SendInvoice && canContact)
        {
            await HandleInvoiceNotificationAsync(existing);
        }
    }

    private async Task HandlePlannedNotificationAsync(Event existing)
    {
        try
        {
            await _emailService.SendEventPlannedNotificationAsync(existing);
            
            // business rule: auto confirm after planned email
            existing.Status = EventStatus.Confirmed;
            existing.NotificationSent = true;
            
            _logger.LogInformation("Planned notification sent for Event {EventId}", existing.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed sending planned notification for Event {EventId}", existing.Id);
        }
    }

    private async Task HandleInvoiceNotificationAsync(Event existing)
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
}