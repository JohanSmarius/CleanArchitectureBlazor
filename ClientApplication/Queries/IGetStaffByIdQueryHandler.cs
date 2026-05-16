using ClientApplication.DataAdapters;
using Entities;

namespace ClientApplication.Queries;

/// <summary>
/// Handles the <see cref="GetStaffByIdQuery"/>.
/// </summary>
public interface IGetStaffByIdQueryHandler
{
    /// <summary>
    /// Returns the staff member with the given identifier, or <c>null</c> if not found.
    /// </summary>
    Task<StaffDTO?> Handle(GetStaffByIdQuery query);
}
