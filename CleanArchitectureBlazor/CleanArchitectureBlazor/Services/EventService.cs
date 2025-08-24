using CleanArchitectureBlazor.Models;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for managing events
/// </summary>
public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task<Event> CreateEventAsync(Event eventModel);
    Task<Event> UpdateEventAsync(Event eventModel);
    Task DeleteEventAsync(int id);
    Task<List<Event>> GetUpcomingEventsAsync();
    Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;

    public EventService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _context.Events
            .Include(e => e.Shifts)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Shifts)
            .ThenInclude(s => s.StaffAssignments)
            .ThenInclude(sa => sa.Staff)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateEventAsync(Event eventModel)
    {
        eventModel.CreatedAt = DateTime.UtcNow;
        _context.Events.Add(eventModel);
        await _context.SaveChangesAsync();
        return eventModel;
    }

    public async Task<Event> UpdateEventAsync(Event eventModel)
    {
        eventModel.UpdatedAt = DateTime.UtcNow;
        _context.Events.Update(eventModel);
        await _context.SaveChangesAsync();
        return eventModel;
    }

    public async Task DeleteEventAsync(int id)
    {
        var eventToDelete = await _context.Events.FindAsync(id);
        if (eventToDelete != null)
        {
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Event>> GetUpcomingEventsAsync()
    {
        var today = DateTime.Today;
        return await _context.Events
            .Include(e => e.Shifts)
            .Where(e => e.StartDate >= today && e.Status != EventStatus.Cancelled)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .Include(e => e.Shifts)
            .Where(e => e.StartDate >= startDate && e.StartDate <= endDate)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }
}
