using Application.DataAdapters;
using Entities;

namespace Application;

public interface IExternalStaffRepository
{
    Task<List<StaffDTO>> GetAllStaffAsync();
    
    Task<StaffDTO?> GetStaffByIdAsync(int id);
    
    Task<StaffDTO> AddStaffAsync(StaffDTO staff);
    
    Task UpdateStaffAsync(StaffDTO staff);
}