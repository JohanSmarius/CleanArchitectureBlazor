using Application.DataAdapters;

using Entities;

namespace Application;

/// <summary>
/// Service for managing shifts
/// </summary>
public interface IShiftRepository
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
