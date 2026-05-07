using Application.Commands;
using Application.DataAdapters;

namespace Application;

/// <summary>
/// Update-event use case that delegates business execution to the update command handler.
/// </summary>
public class UpdateEventUseCase : IUpdateEventUseCase
{
    private readonly IUpdateEventCommandHandler _updateEventCommandHandler;

    /// <summary>
    /// Initialises a new instance of <see cref="UpdateEventUseCase"/>.
    /// </summary>
    public UpdateEventUseCase(IUpdateEventCommandHandler updateEventCommandHandler)
    {
        _updateEventCommandHandler = updateEventCommandHandler;
    }

    /// <inheritdoc />
    public Task<EventDTO> Execute(EventDTO updated)
        => _updateEventCommandHandler.Handle(updated.ToUpdateCommand());
}
