namespace DomainService;

public interface IExternalStaffRepository
{
    Task<List<Staff>> GetAllStaffAsync();
    
    Task<Staff?> GetStaffByIdAsync(int id);
    
    Task<Staff> AddStaffAsync(Staff staff);
    
    Task UpdateStaffAsync(Staff staff);
}