using Entities;

namespace Application;

/// <summary>
/// Provides staff CRUD operations against the external staff API.
/// </summary>
public interface IExternalStaffRepository
{
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff?> GetStaffByIdAsync(int id);
    Task<Staff> AddStaffAsync(Staff staff);
    Task UpdateStaffAsync(Staff staff);
}
