using Domain;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing staff
/// </summary>
public interface IStaffRepository
{
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff?> GetStaffByIdAsync(int id);
    Task<Staff> CreateStaffAsync(Domain.Staff staff);
    Task<Staff> UpdateStaffAsync(Domain.Staff staff);
    Task DeleteStaffAsync(int id);
    Task<List<Domain.Staff>> GetActiveStaffAsync();
    Task<List<Domain.Staff>> GetStaffByRoleAsync(StaffRole role);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
}
