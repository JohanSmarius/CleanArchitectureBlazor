using Entities;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _context;

    public ShiftRepository(ApplicationDbContext context)
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

        // Find the existing entity in the context
        var existingShift = await _context.Shifts.FindAsync(shift.Id);
        if (existingShift == null)
        {
            throw new InvalidOperationException($"Shift with ID {shift.Id} not found.");
        }

        // Update only the scalar properties to avoid navigation property tracking conflicts
        _context.Entry(existingShift).CurrentValues.SetValues(shift);

        await _context.SaveChangesAsync();
        return existingShift;
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
