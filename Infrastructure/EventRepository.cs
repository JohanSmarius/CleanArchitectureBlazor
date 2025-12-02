using Application;
using Entities;
using CleanArchitectureBlazor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(ApplicationDbContext context, ILogger<EventRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        var existingEvent = await _context.Events.FindAsync(eventModel.Id);
        if (existingEvent == null)
        {
            throw new InvalidOperationException($"Event with ID {eventModel.Id} not found.");
        }

        existingEvent.Name = eventModel.Name;
        existingEvent.StartDate = eventModel.StartDate;
        existingEvent.EndDate = eventModel.EndDate;
        existingEvent.Location = eventModel.Location;
        existingEvent.Description = eventModel.Description;
        existingEvent.Status = eventModel.Status;
        existingEvent.ContactPerson = eventModel.ContactPerson;
        existingEvent.ContactPhone = eventModel.ContactPhone;
        existingEvent.ContactEmail = eventModel.ContactEmail;
        existingEvent.NotificationSent = eventModel.NotificationSent;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingEvent;
    }

    public async Task DeleteEventAsync(int id)
    {
        // Load event with related shifts and assignments
        var eventToDelete = await _context.Events
            .Include(e => e.Shifts)
            .ThenInclude(s => s.StaffAssignments)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventToDelete != null)
        {
            // Delete all staff assignments for shifts (do not delete Staff entities)
            var allAssignments = eventToDelete.Shifts.SelectMany(s => s.StaffAssignments).ToList();
            if (allAssignments.Any())
            {
                _context.StaffAssignments.RemoveRange(allAssignments);
            }

            // Delete all shifts for the event
            if (eventToDelete.Shifts.Any())
            {
                _context.Shifts.RemoveRange(eventToDelete.Shifts);
            }

            // Finally remove the event itself
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
