using Application.Commands;
using Application.DataAdapters;

namespace Application;

/// <summary>
/// Create-event use case that delegates business execution to the create command handler.
/// </summary>
public class CreateEventUseCase : ICreateEventUseCase
{
    private readonly ICreateEventCommandHandler _createEventCommandHandler;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateEventUseCase"/>.
    /// </summary>
    public CreateEventUseCase(ICreateEventCommandHandler createEventCommandHandler)
    {
        _createEventCommandHandler = createEventCommandHandler;
    }

    /// <inheritdoc />
    public Task<EventDTO> Execute(EventDTO newEvent)
        => _createEventCommandHandler.Handle(newEvent.ToCreateCommand());
}
