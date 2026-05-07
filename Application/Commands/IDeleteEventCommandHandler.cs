namespace Application.Commands;

/// <summary>
/// Handles the <see cref="DeleteEventCommand"/>.
/// </summary>
public interface IDeleteEventCommandHandler
{
    /// <summary>
    /// Deletes the event with the identifier specified in the command.
    /// </summary>
    /// <param name="command">The command containing the event identifier.</param>
    Task Handle(DeleteEventCommand command);
}
