namespace Application.Commands;

/// <summary>
/// Handles the <see cref="DeleteStaffCommand"/>.
/// </summary>
public interface IDeleteStaffCommandHandler
{
    /// <summary>
    /// Deactivates the staff member with the identifier specified in the command.
    /// </summary>
    Task Handle(DeleteStaffCommand command);
}
