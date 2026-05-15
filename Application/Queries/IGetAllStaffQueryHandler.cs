using Entities;

namespace Application.Queries;

/// <summary>
/// Handles the <see cref="GetAllStaffQuery"/>.
/// </summary>
public interface IGetAllStaffQueryHandler
{
    /// <summary>
    /// Returns all staff members.
    /// </summary>
    Task<List<Staff>> Handle(GetAllStaffQuery query);
}
