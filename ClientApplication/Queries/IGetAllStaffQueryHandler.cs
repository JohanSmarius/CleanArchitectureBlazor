using ClientApplication.DataAdapters;
using Entities;

namespace ClientApplication.Queries;

/// <summary>
/// Handles the <see cref="GetAllStaffQuery"/>.
/// </summary>
public interface IGetAllStaffQueryHandler
{
    /// <summary>
    /// Returns all staff members.
    /// </summary>
    Task<List<StaffDTO>> Handle(GetAllStaffQuery query);
}
