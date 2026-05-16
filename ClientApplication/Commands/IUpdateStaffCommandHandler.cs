using Entities;

namespace ClientApplication.Commands;

/// <summary>
/// Handles the <see cref="UpdateStaffCommand"/>.
/// </summary>
public interface IUpdateStaffCommandHandler
{
    /// <summary>
    /// Updates a staff member from the specified command.
    /// </summary>
    Task Handle(UpdateStaffCommand command);
}
