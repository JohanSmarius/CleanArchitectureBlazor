using CleanArchitectureBlazor.Data;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<EventService> _logger;

    public EventService(ApplicationDbContext context, IEmailService emailService, ILogger<EventService> logger)
    {
        _context = context;
        _emailService = emailService;
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

        // Store the original status to check for changes
        var originalStatus = existingEvent.Status;
        var originalNotificationSent = existingEvent.NotificationSent;

        // Update the entity
        eventModel.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingEvent).CurrentValues.SetValues(eventModel);

        // Check if status changed to Planned and email hasn't been sent
        if (originalStatus != EventStatus.Planned && 
            eventModel.Status == EventStatus.Planned && 
            !originalNotificationSent &&
            !string.IsNullOrEmpty(eventModel.ContactEmail))
        {
            try
            {
                // Send planned notification email
                await _emailService.SendEventPlannedNotificationAsync(eventModel);
                
                // Mark notification as sent but keep status as Planned
                existingEvent.Status = EventStatus.Confirmed;
                existingEvent.NotificationSent = true;
                
                _logger.LogInformation("Event planned email sent for event {EventId} to {ContactEmail}", 
                    eventModel.Id, eventModel.ContactEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event planned email for event {EventId}", eventModel.Id);
                // Don't throw - we still want to save the event status change
            }
        }

        // Check if status changed to SendInvoice
        if (originalStatus != EventStatus.SendInvoice && 
            eventModel.Status == EventStatus.SendInvoice && 
            !string.IsNullOrEmpty(eventModel.ContactEmail))
        {
            try
            {
                // Send invoice notification email
                await _emailService.SendEventInvoiceNotificationAsync(eventModel);
                
                // Mark notification as sent and keep status as SendInvoice
                existingEvent.NotificationSent = true;
                
                _logger.LogInformation("Event invoice email sent for event {EventId} to {ContactEmail}", 
                    eventModel.Id, eventModel.ContactEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event invoice email for event {EventId}", eventModel.Id);
                // Don't throw - we still want to save the event status change
            }
        }

        await _context.SaveChangesAsync();
        return existingEvent;
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
