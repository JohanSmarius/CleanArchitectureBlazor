using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="DeleteEventCommand"/> by delegating to the command-side repository.
/// </summary>
public class DeleteEventCommandHandler : IDeleteEventCommandHandler
{
    private readonly IEventCommandRepository _commandRepository;
    private readonly ILogger<DeleteEventCommandHandler> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="DeleteEventCommandHandler"/>.
    /// </summary>
    public DeleteEventCommandHandler(
        IEventCommandRepository commandRepository,
        ILogger<DeleteEventCommandHandler> logger)
    {
        _commandRepository = commandRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteEventCommand command)
    {
        await _commandRepository.DeleteEventAsync(command.Id);
        _logger.LogInformation("Event {EventId} deleted successfully.", command.Id);
    }
}
