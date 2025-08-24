using CleanArchitectureBlazor.Models;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing shifts
/// </summary>
public interface IShiftService
{
    Task<List<Shift>> GetAllShiftsAsync();
    Task<Shift?> GetShiftByIdAsync(int id);
    Task<List<Shift>> GetShiftsByEventIdAsync(int eventId);
    Task<Shift> CreateShiftAsync(Shift shift);
    Task<Shift> UpdateShiftAsync(Shift shift);
    Task DeleteShiftAsync(int id);
    Task<List<Shift>> GetUpcomingShiftsAsync();
    Task<List<Shift>> GetShiftsByDateAsync(DateTime date);
}

public class ShiftService : IShiftService
{
    private readonly ApplicationDbContext _context;

    public ShiftService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shift>> GetAllShiftsAsync()
    {
        return await _context.Shifts
            .Include(s => s.Event)
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Shift?> GetShiftByIdAsync(int id)
    {
        return await _context.Shifts
            .Include(s => s.Event)
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Shift>> GetShiftsByEventIdAsync(int eventId)
    {
        return await _context.Shifts
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .Where(s => s.EventId == eventId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Shift> CreateShiftAsync(Shift shift)
    {
        shift.CreatedAt = DateTime.UtcNow;
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task<Shift> UpdateShiftAsync(Shift shift)
    {
        shift.UpdatedAt = DateTime.UtcNow;
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task DeleteShiftAsync(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);
        if (shift != null)
        {
            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Shift>> GetUpcomingShiftsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Shifts
            .Include(s => s.Event)
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .Where(s => s.StartTime >= now && s.Status != ShiftStatus.Cancelled)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Shift>> GetShiftsByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.Shifts
            .Include(s => s.Event)
            .Include(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }
}
