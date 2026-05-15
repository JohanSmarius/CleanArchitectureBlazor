using Entities;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="UpdateStaffCommand"/>.
/// </summary>
public interface IUpdateStaffCommandHandler
{
    /// <summary>
    /// Updates a staff member from the specified command.
    /// </summary>
    Task<Staff> Handle(UpdateStaffCommand command);
}
