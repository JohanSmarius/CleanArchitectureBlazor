using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetStaffByIdQuery"/>.
/// </summary>
public interface IGetStaffByIdQueryHandler
{
    /// <summary>
    /// Returns the staff member with the given identifier, or <c>null</c> if not found.
    /// </summary>
    Task<Staff?> Handle(GetStaffByIdQuery query);
}
