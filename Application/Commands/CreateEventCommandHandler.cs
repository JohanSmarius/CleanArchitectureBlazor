using Application.DataAdapters;
using Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="CreateEventCommand"/> by validating the input, building the event
/// entity, and persisting it via the command-side repository.
/// </summary>
public class CreateEventCommandHandler : ICreateEventCommandHandler
{
    private readonly IEventCommandRepository _commandRepository;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateEventCommandHandler"/>.
    /// </summary>
    /// <param name="commandRepository">Write-side repository for event persistence.</param>
    /// <param name="logger">Logger instance.</param>
    public CreateEventCommandHandler(
        IEventCommandRepository commandRepository,
        ILogger<CreateEventCommandHandler> logger)
    {
        _commandRepository = commandRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<EventDTO> Handle(CreateEventCommand command)
    {
        // Validate dates
        if (command.StartDate >= command.EndDate)
        {
            throw new ApplicationException("End date must be after start date.");
        }

        if (command.StartDate <= DateTime.UtcNow)
        {
            throw new ApplicationException("Start date must be in the future.");
        }

        var entity = new Event
        {
            Name = command.Name,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Location = command.Location,
            Description = command.Description,
            ContactPerson = command.ContactPerson,
            ContactPhone = command.ContactPhone,
            ContactEmail = command.ContactEmail,
            Status = EventStatus.Requested,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Create a shift to cover the entire event duration by default
        entity.Shifts.Add(new Shift
        {
            Name = "Default Shift",
            StartTime = command.StartDate,
            EndTime = command.EndDate,
            RequiredStaff = 1,
            Description = "Default shift covering the entire event duration",
            Status = ShiftStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });

        var createdEvent = await _commandRepository.CreateEventAsync(entity);

        _logger.LogInformation("Event {EventId} created successfully.", createdEvent.Id);

        return createdEvent.ToDTO();
    }
}
