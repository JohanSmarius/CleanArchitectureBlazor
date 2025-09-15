using Domain;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing staff
/// </summary>
public interface IStaffService
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

public class StaffService : IStaffService
{
    private readonly ApplicationDbContext _context;

    public StaffService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Domain.Staff>> GetAllStaffAsync()
    {
        return await _context.Staff
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<Domain.Staff?> GetStaffByIdAsync(int id)
    {
        return await _context.Staff
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Shift)
            .ThenInclude(s => s.Event)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Domain.Staff> CreateStaffAsync(Staff staff)
    {
        staff.CreatedAt = DateTime.UtcNow;
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Domain.Staff> UpdateStaffAsync(Staff staff)
    {
        staff.UpdatedAt = DateTime.UtcNow;
        _context.Staff.Update(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task DeleteStaffAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            // Soft delete by setting IsActive to false
            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Domain.Staff>> GetActiveStaffAsync()
    {
        return await _context.Staff
            .Where(s => s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<List<Staff>> GetStaffByRoleAsync(StaffRole role)
    {
        return await _context.Staff
            .Where(s => s.Role == role && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        var query = _context.Staff.Where(s => s.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }
}
