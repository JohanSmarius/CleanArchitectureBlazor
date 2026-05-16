using ClientApplication.DataAdapters;

namespace ClientApplication;

/// <summary>
/// Provides staff CRUD operations against the external staff API.
/// </summary>
public interface IExternalStaffRepository
{
    Task<List<StaffDTO>> GetAllStaffAsync();
    Task<StaffDTO?> GetStaffByIdAsync(int id);
    Task<StaffDTO> AddStaffAsync(StaffDTO staff);
    Task UpdateStaffAsync(StaffDTO staff);
}
