using CleanArchitectureBlazor.Data;
using Domain;
using DomainService;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _context;

    public StaffRepository(ApplicationDbContext context)
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
