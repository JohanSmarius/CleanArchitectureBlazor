using CleanArchitectureBlazor.Models;
using CleanArchitectureBlazor.Repositories;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing staff
/// </summary>
public interface IStaffService
{
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff?> GetStaffByIdAsync(int id);
    Task<Staff> CreateStaffAsync(Staff staff);
    Task<Staff> UpdateStaffAsync(Staff staff);
    Task DeleteStaffAsync(int id);
    Task<List<Staff>> GetActiveStaffAsync();
    Task<List<Staff>> GetStaffByRoleAsync(StaffRole role);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
}

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await _staffRepository.GetAllAsync();
    }

    public async Task<Staff?> GetStaffByIdAsync(int id)
    {
        return await _staffRepository.GetByIdAsync(id);
    }

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        return await _staffRepository.CreateAsync(staff);
    }

    public async Task<Staff> UpdateStaffAsync(Staff staff)
    {
        return await _staffRepository.UpdateAsync(staff);
    }

    public async Task DeleteStaffAsync(int id)
    {
        await _staffRepository.DeleteAsync(id);
    }

    public async Task<List<Staff>> GetActiveStaffAsync()
    {
        return await _staffRepository.GetActiveAsync();
    }

    public async Task<List<Staff>> GetStaffByRoleAsync(StaffRole role)
    {
        return await _staffRepository.GetByRoleAsync(role);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return await _staffRepository.IsEmailUniqueAsync(email, excludeId);
    }
}
