using CleanArchitectureBlazor.Models;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a staff assignment notification email
    /// </summary>
    /// <param name="staff">The staff member being assigned</param>
    /// <param name="shift">The shift being assigned to</param>
    /// <param name="event">The event containing the shift</param>
    Task SendStaffAssignmentNotificationAsync(Staff staff, Shift shift, Event @event);
    
    /// <summary>
    /// Sends an event planned notification email to the contact person
    /// </summary>
    /// <param name="event">The event that has been planned</param>
    Task SendEventPlannedNotificationAsync(Event @event);
    
    /// <summary>
    /// Sends an event confirmation email to the contact person
    /// </summary>
    /// <param name="event">The event being confirmed</param>
    Task SendEventConfirmationNotificationAsync(Event @event);
    
    /// <summary>
    /// Sends a general email
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlBody">HTML email body</param>
    Task SendEmailAsync(string to, string subject, string htmlBody);
}