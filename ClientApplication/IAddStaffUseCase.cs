using ClientApplication.DataAdapters;

namespace ClientApplication;

/// <summary>
/// Use case for adding a new staff member.
/// </summary>
public interface IAddStaffUseCase
{
    Task<StaffDTO> ExecuteAsync(StaffDTO staff);
}
