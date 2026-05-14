using CleanArchitectureBlazor.Data;
using CleanArchitectureBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Repositories;

/// <summary>
/// Repository abstraction for staff persistence operations.
/// </summary>
public interface IStaffRepository
{
    Task<List<Staff>> GetAllAsync();
    Task<Staff?> GetByIdAsync(int id);
    Task<Staff> CreateAsync(Staff staff);
    Task<Staff> UpdateAsync(Staff staff);
    Task DeleteAsync(int id);
    Task<List<Staff>> GetActiveAsync();
    Task<List<Staff>> GetByRoleAsync(StaffRole role);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
}

/// <summary>
/// EF Core-backed repository for staff persistence.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Creates a new staff repository.
    /// </summary>
    public StaffRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staff
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staff
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Shift)
            .ThenInclude(s => s.Event)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Staff> CreateAsync(Staff staff)
    {
        staff.CreatedAt = DateTime.UtcNow;
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Staff> UpdateAsync(Staff staff)
    {
        staff.UpdatedAt = DateTime.UtcNow;
        _context.Staff.Update(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task DeleteAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            // Soft delete by setting IsActive to false.
            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Staff>> GetActiveAsync()
    {
        return await _context.Staff
            .Where(s => s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<List<Staff>> GetByRoleAsync(StaffRole role)
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