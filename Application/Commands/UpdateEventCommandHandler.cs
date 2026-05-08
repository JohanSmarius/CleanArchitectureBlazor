using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="UpdateEventCommand"/> by validating the input, applying changes, and persisting.
/// </summary>
public class UpdateEventCommandHandler : IUpdateEventCommandHandler
{
    private readonly IEventQueryRepository _queryRepository;
    private readonly IEventCommandRepository _commandRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdateEventCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="UpdateEventCommandHandler"/>.
    /// </summary>
    public UpdateEventCommandHandler(
        IEventQueryRepository queryRepository,
        IEventCommandRepository commandRepository,
        IEmailService emailService,
        ILogger<UpdateEventCommandHandler> logger)
    {
        _queryRepository = queryRepository;
        _commandRepository = commandRepository;
        _emailService = emailService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<EventDTO> Handle(UpdateEventCommand command)
    {
        // Validate dates
        if (command.StartDate >= command.EndDate)
        {
            throw new ApplicationException("End date must be after start date.");
        }

        // Load current state
        var existing = await _queryRepository.GetEventByIdAsync(command.Id) ??
            throw new InvalidOperationException($"Event {command.Id} not found");

        if (existing.Id != command.Id)
            throw new InvalidOperationException("Mismatched Event ids.");

        // Check if date changes affect existing shifts
        if (command.Shifts.Any() &&
            (command.StartDate != existing.StartDate || command.EndDate != existing.EndDate))
        {
            var conflictingShifts = command.Shifts.Where(s =>
                s.StartTime < command.StartDate || s.EndTime > command.EndDate).ToList();

            if (conflictingShifts.Any())
            {
                throw new DomainException(
                    $"Cannot change event dates. {conflictingShifts.Count} shift(s) would fall outside the new event timeframe.");
            }
        }

        // Capture original values needed for rules
        var originalStatus = existing.Status;
        var originalNotificationSent = existing.NotificationSent;

        // Copy over mutable fields (avoid overwriting identity / audit)
        existing.Name = command.Name;
        existing.StartDate = command.StartDate;
        existing.EndDate = command.EndDate;
        existing.Location = command.Location;
        existing.Description = command.Description;
        existing.Status = (EventStatus)command.Status;
        existing.ContactPerson = command.ContactPerson;
        existing.ContactPhone = command.ContactPhone;
        existing.ContactEmail = command.ContactEmail;
        existing.UpdatedAt = DateTime.UtcNow;

        bool canContact = !string.IsNullOrWhiteSpace(existing.ContactEmail);

        bool shouldSendPlanned =
            originalStatus != EventStatus.Planned &&
            existing.Status == EventStatus.Planned &&
            !originalNotificationSent &&
            canContact;

        bool shouldSendInvoice =
            originalStatus != EventStatus.SendInvoice &&
            existing.Status == EventStatus.SendInvoice &&
            canContact;

        // Perform side-effects (email notifications)
        if (shouldSendPlanned)
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
        await _commandRepository.UpdateEventAsync(existing);

        return existing.ToDTO();
    }
}
