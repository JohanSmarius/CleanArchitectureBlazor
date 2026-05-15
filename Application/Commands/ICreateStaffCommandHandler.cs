using Entities;

namespace Application.Commands;

/// <summary>
/// Handles the <see cref="CreateStaffCommand"/>.
/// </summary>
public interface ICreateStaffCommandHandler
{
    /// <summary>
    /// Creates a staff member from the specified command.
    /// </summary>
    Task<Staff> Handle(CreateStaffCommand command);
}
