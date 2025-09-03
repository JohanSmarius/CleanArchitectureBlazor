using CleanArchitectureBlazor.Models;
using CleanArchitectureBlazor.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// SMTP-based email service for sending notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends a staff assignment notification email
    /// </summary>
    public async Task SendStaffAssignmentNotificationAsync(Staff staff, Shift shift, Event @event)
    {
        var subject = $"Shift Assignment: {@event.Name}";
        var htmlBody = GenerateAssignmentEmailBody(staff, shift, @event);
        
        await SendEmailAsync(staff.Email, subject, htmlBody);
    }

    /// <summary>
    /// Sends an event planned notification email to the contact person
    /// </summary>
    public async Task SendEventPlannedNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(@event.ContactEmail))
        {
            _logger.LogWarning("Cannot send event planned email for event {EventId}: No contact email provided", @event.Id);
            return;
        }

        var subject = $"Event Planning Update: {@event.Name}";
        var htmlBody = GenerateEventPlannedEmailBody(@event);
        
        await SendEmailAsync(@event.ContactEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends an event confirmation notification email to the contact person
    /// </summary>
    public async Task SendEventConfirmationNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(@event.ContactEmail))
        {
            _logger.LogWarning("Cannot send event confirmation email for event {EventId}: No contact email provided", @event.Id);
            return;
        }

        var subject = $"Event Confirmed: {@event.Name}";
        var htmlBody = GenerateEventConfirmationEmailBody(@event);
        
        await SendEmailAsync(@event.ContactEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends an invoice notification email to the contact person
    /// </summary>
    public async Task SendEventInvoiceNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(@event.ContactEmail))
        {
            _logger.LogWarning("Cannot send event invoice email for event {EventId}: No contact email provided", @event.Id);
            return;
        }

        var subject = $"Invoice for Event: {@event.Name}";
        var htmlBody = GenerateEventInvoiceEmailBody(@event);
        
        await SendEmailAsync(@event.ContactEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends an invoice notification email to the financial department
    /// </summary>
    public async Task SendFinancialDepartmentInvoiceNotificationAsync(Event @event)
    {
        if (string.IsNullOrEmpty(_emailOptions.FinancialDepartmentEmail))
        {
            _logger.LogWarning("Cannot send invoice notification to financial department for event {EventId}: Financial department email not configured", @event.Id);
            return;
        }

        var subject = $"Invoice Required: {@event.Name} - Event ID: {@event.Id}";
        var htmlBody = GenerateFinancialDepartmentInvoiceEmailBody(@event);
        
        await SendEmailAsync(_emailOptions.FinancialDepartmentEmail, subject, htmlBody);
    }

    /// <summary>
    /// Sends a general email using SMTP
    /// </summary>
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var host = _emailOptions.SmtpHost;
            var port = _emailOptions.SmtpPort;
            var username = _emailOptions.SmtpUsername;
            var password = _emailOptions.SmtpPassword;
            var fromEmail = _emailOptions.FromEmail;
            var fromName = _emailOptions.FromName;
            var enableSsl = _emailOptions.EnableSsl;

            if (string.IsNullOrEmpty(host)) // || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", to);
                return;
            }

            using var client = new SmtpClient(host, port);

            if (enableSsl)
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(username, password);
            }
            else
            {
            }

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail ?? username, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // Don't throw to prevent blocking the assignment creation
        }
    }

    /// <summary>
    /// Generates the HTML body for staff assignment notification email
    /// </summary>
    private string GenerateAssignmentEmailBody(Staff staff, Shift shift, Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Shift Assignment Notification</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #007bff; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-success { background: #28a745; color: white; }");
        sb.AppendLine("        .badge-info { background: #17a2b8; color: white; }");
        sb.AppendLine("        .badge-warning { background: #ffc107; color: #212529; }");
        sb.AppendLine("        .badge-danger { background: #dc3545; color: white; }");
        sb.AppendLine("        .badge-primary { background: #007bff; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Shift Assignment Notification</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        sb.AppendLine($"            <p>Hello <strong>{staff.FullName}</strong>,</p>");
        sb.AppendLine("            <p>You have been assigned to a new shift. Please review the details below:</p>");
        
        // Event Details
        sb.AppendLine("            <h3>?? Event Details</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Name:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Location:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Duration:</div>");
        sb.AppendLine($"                <div>?? {@event.StartDate:MMM dd, yyyy HH:mm} - {@event.EndDate:MMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Contact:</div>");
            sb.AppendLine($"                <div>?? {@event.ContactPerson}");
            if (!string.IsNullOrEmpty(@event.ContactPhone))
            {
                sb.AppendLine($" - ?? {@event.ContactPhone}");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("            </div>");
        }
        
        // Shift Details
        sb.AppendLine("            <h3>? Your Shift Details</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Shift Name:</div>");
        sb.AppendLine($"                <div>{shift.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Shift Time:</div>");
        sb.AppendLine($"                <div>?? {shift.StartTime:MMM dd, yyyy HH:mm} - {shift.EndTime:MMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        var duration = shift.EndTime - shift.StartTime;
        var durationText = duration.TotalHours >= 24 
            ? $"{duration.Days}d {duration.Hours}h" 
            : $"{duration.Hours}h {duration.Minutes}m";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duration:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(shift.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Shift Description:</div>");
            sb.AppendLine($"                <div>{shift.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Staff Role Badge
        var roleColor = staff.Role switch
        {
            StaffRole.Doctor => "badge-danger",
            StaffRole.Paramedic => "badge-warning",
            StaffRole.TeamLeader => "badge-primary",
            StaffRole.FirstAider => "badge-success",
            StaffRole.Volunteer => "badge-info",
            _ => "badge-info"
        };
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Your Role:</div>");
        sb.AppendLine($"                <div><span class='badge {roleColor}'>{staff.Role}</span></div>");
        sb.AppendLine("            </div>");
        
        // Important Notes
        sb.AppendLine("            <h3>?? Important Notes</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Please arrive <strong>15 minutes early</strong> for briefing</li>");
        sb.AppendLine("                <li>Bring your certification documents and ID</li>");
        sb.AppendLine("                <li>Wear appropriate medical/first aid attire</li>");
        sb.AppendLine("                <li>Contact the event organizer if you cannot attend</li>");
        sb.AppendLine("            </ul>");
        
        sb.AppendLine("            <p><strong>Thank you for your service!</strong></p>");
        sb.AppendLine("            <p>If you have any questions about this assignment, please contact the event organizer.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>This is an automated notification from the Medical First Aid Event Manager.</p>");
        sb.AppendLine($"            <p>Email sent on {DateTime.UtcNow:MMM dd, yyyy HH:mm} UTC</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event planned notification email
    /// </summary>
    private string GenerateEventPlannedEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Event Planning Update</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #007bff; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-primary { background: #007bff; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #d1ecf1; padding: 15px; border-radius: 4px; border-left: 4px solid #007bff; margin: 15px 0; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Event Planning Update</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Hello <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Hello,</p>");
        }
        
        sb.AppendLine("            <p>Good news! We have started planning the medical first aid coverage for your event.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Your Event is Now Being Planned</h3>");
        sb.AppendLine("                <p>Our team has reviewed your event request and we are now in the planning phase. This means we are working on:</p>");
        sb.AppendLine("                <ul>");
        sb.AppendLine("                    <li>Assessing the medical coverage requirements for your event</li>");
        sb.AppendLine("                    <li>Scheduling appropriate medical staff</li>");
        sb.AppendLine("                    <li>Preparing necessary medical equipment and supplies</li>");
        sb.AppendLine("                    <li>Planning the first aid station setup</li>");
        sb.AppendLine("                </ul>");
        sb.AppendLine("            </div>");
        
        // Event Details (using the rest of the existing implementation)
        sb.AppendLine("            <h3>?? Event Details</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Name:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Location:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Duration:</div>");
        sb.AppendLine($"                <div>?? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Current Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Planned</span></div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Thank you for choosing our medical first aid services!</strong></p>");
        sb.AppendLine("            <p>We are committed to providing professional and reliable medical coverage for your event.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>This is an automated notification from the Medical First Aid Event Manager.</p>");
        sb.AppendLine($"            <p>Planning notification sent on {DateTime.UtcNow:MMMM dd, yyyy 'at' HH:mm} UTC</p>");
        sb.AppendLine($"            <p>Reference ID: EVENT-{@event.Id:D6}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event confirmation notification email
    /// </summary>
    private string GenerateEventConfirmationEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Event Confirmation</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #28a745; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-success { background: #28a745; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #fff3cd; padding: 15px; border-radius: 4px; border-left: 4px solid #ffc107; margin: 15px 0; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>? Event Confirmation</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Hello <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Hello,</p>");
        }
        
        sb.AppendLine("            <p>Great news! Your event request has been <strong>confirmed</strong> by our medical first aid team.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>? Your Event is Confirmed!</h3>");
        sb.AppendLine("                <p>We are pleased to confirm that medical first aid coverage has been arranged for your event.</p>");
        sb.AppendLine("            </div>");
        
        // Event Details (simplified version)
        sb.AppendLine("            <h3>?? Event Details</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Name:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Location:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Date & Time:</div>");
        sb.AppendLine($"                <div>?? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-success'>? Confirmed</span></div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Thank you for choosing our medical first aid services!</strong></p>");
        sb.AppendLine("            <p>We look forward to providing professional medical coverage for your event.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>This is an automated confirmation from the Medical First Aid Event Manager.</p>");
        sb.AppendLine($"            <p>Confirmation sent on {DateTime.UtcNow:MMMM dd, yyyy 'at' HH:mm} UTC</p>");
        sb.AppendLine($"            <p>Reference ID: EVENT-{@event.Id:D6}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for event invoice notification email
    /// </summary>
    private string GenerateEventInvoiceEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Event Invoice</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #6f42c1; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-primary { background: #6f42c1; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #e7d3ff; padding: 15px; border-radius: 4px; border-left: 4px solid #6f42c1; margin: 15px 0; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Event Invoice</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine($"            <p>Dear <strong>{@event.ContactPerson}</strong>,</p>");
        }
        else
        {
            sb.AppendLine("            <p>Dear Valued Customer,</p>");
        }
        
        sb.AppendLine("            <p>Thank you for choosing our medical first aid services for your event. Your event has been successfully completed, and we are now sending you the invoice for the services provided.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Invoice Ready for Your Event</h3>");
        sb.AppendLine("                <p>Our medical first aid team successfully provided coverage for your event. Please find the invoice details below and remit payment according to our terms.</p>");
        sb.AppendLine("            </div>");
        
        // Event Details (simplified)
        sb.AppendLine("            <h3>?? Event Details</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Name:</div>");
        sb.AppendLine($"                <div>{@event.Name}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Location:</div>");
        sb.AppendLine($"                <div>?? {@event.Location}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Date & Time:</div>");
        sb.AppendLine($"                <div>?? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Invoice Sent</span></div>");
        sb.AppendLine("            </div>");
        
        // Invoice Information
        sb.AppendLine("            <h3>?? Invoice Information</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Invoice Number:</div>");
        sb.AppendLine($"                <div>INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Invoice Date:</div>");
        sb.AppendLine($"                <div>{DateTime.UtcNow:MMMM dd, yyyy}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Service Period:</div>");
        sb.AppendLine($"                <div>{@event.StartDate:MMM dd, yyyy} - {@event.EndDate:MMM dd, yyyy}</div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Thank you for choosing our medical first aid services!</strong></p>");
        sb.AppendLine("            <p>We appreciate your business and hope you were satisfied with our professional medical coverage. Please don't hesitate to contact us for future events.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>This invoice was automatically generated by the Medical First Aid Event Manager.</p>");
        sb.AppendLine($"            <p>Invoice sent on {DateTime.UtcNow:MMMM dd, yyyy 'at' HH:mm} UTC</p>");
        sb.AppendLine($"            <p>Reference ID: EVENT-{@event.Id:D6} | Invoice: INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the HTML body for financial department invoice notification email
    /// </summary>
    private string GenerateFinancialDepartmentInvoiceEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Invoice Required - Financial Department</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 700px; margin: 0 auto; background: #f9f9f9; padding: 20px; border-radius: 8px; }");
        sb.AppendLine("        .header { background: #dc3545; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }");
        sb.AppendLine("        .content { background: white; padding: 20px; border-radius: 0 0 8px 8px; }");
        sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine("        .badge-danger { background: #dc3545; color: white; }");
        sb.AppendLine("        .badge-success { background: #28a745; color: white; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; padding: 10px; background: #f8f9fa; border-radius: 4px; }");
        sb.AppendLine("        .detail-label { font-weight: bold; color: #495057; margin-bottom: 5px; }");
        sb.AppendLine("        .detail-value { color: #212529; }");
        sb.AppendLine("        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #6c757d; }");
        sb.AppendLine("        .highlight { background: #f8d7da; padding: 15px; border-radius: 4px; border-left: 4px solid #dc3545; margin: 15px 0; }");
        sb.AppendLine("        .invoice-summary { background: #fff3cd; padding: 15px; border-radius: 4px; border-left: 4px solid #ffc107; margin: 15px 0; }");
        sb.AppendLine("        .action-required { background: #d1ecf1; padding: 15px; border-radius: 4px; border-left: 4px solid #17a2b8; margin: 15px 0; }");
        sb.AppendLine("        .event-table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
        sb.AppendLine("        .event-table th, .event-table td { padding: 8px; text-align: left; border-bottom: 1px solid #dee2e6; }");
        sb.AppendLine("        .event-table th { background-color: #f8f9fa; font-weight: bold; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine("            <h1>?? Invoice Required - Medical First Aid Event</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='content'>");
        
        sb.AppendLine("            <p><strong>Dear Financial Department,</strong></p>");
        sb.AppendLine("            <p>An event has been completed and is ready for invoicing. Please prepare and send an invoice to the customer for the medical first aid services provided.</p>");
        
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <h3>?? Action Required</h3>");
        sb.AppendLine("                <p><strong>Please prepare and send an invoice for the completed event below.</strong></p>");
        sb.AppendLine("            </div>");
        
        // Event Details Table
        sb.AppendLine("            <h3>?? Event Details</h3>");
        sb.AppendLine("            <table class='event-table'>");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Event ID</th>");
        sb.AppendLine($"                    <td>EVENT-{@event.Id:D6}</td>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Event Name</th>");
        sb.AppendLine($"                    <td>{@event.Name}</td>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Location</th>");
        sb.AppendLine($"                    <td>?? {@event.Location}</td>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Event Date & Time</th>");
        sb.AppendLine($"                    <td>?? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</td>");
        sb.AppendLine("                </tr>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} day(s), {duration.Hours} hour(s)" 
            : $"{duration.Hours} hour(s), {duration.Minutes} minute(s)";
        
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Event Duration</th>");
        sb.AppendLine($"                    <td>?? {durationText}</td>");
        sb.AppendLine("                </tr>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("                <tr>");
            sb.AppendLine("                    <th>Event Description</th>");
            sb.AppendLine($"                    <td>{@event.Description}</td>");
            sb.AppendLine("                </tr>");
        }
        
        sb.AppendLine("                <tr>");
        sb.AppendLine("                    <th>Current Status</th>");
        sb.AppendLine($"                    <td><span class='badge badge-danger'>?? Send Invoice</span></td>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("            </table>");
        
        // Customer Contact Information
        sb.AppendLine("            <h3>?? Customer Contact Information</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        if (!string.IsNullOrEmpty(@event.ContactPerson))
        {
            sb.AppendLine("                <div class='detail-label'>Contact Person:</div>");
            sb.AppendLine($"                <div class='detail-value'>?? {@event.ContactPerson}</div>");
        }
        if (!string.IsNullOrEmpty(@event.ContactEmail))
        {
            sb.AppendLine("                <div class='detail-label'>Email Address:</div>");
            sb.AppendLine($"                <div class='detail-value'>?? <a href='mailto:{@event.ContactEmail}'>{@event.ContactEmail}</a></div>");
        }
        if (!string.IsNullOrEmpty(@event.ContactPhone))
        {
            sb.AppendLine("                <div class='detail-label'>Phone Number:</div>");
            sb.AppendLine($"                <div class='detail-value'>?? {@event.ContactPhone}</div>");
        }
        sb.AppendLine("            </div>");
        
        // Invoice Summary Information
        sb.AppendLine("            <div class='invoice-summary'>");
        sb.AppendLine("                <h3>?? Invoice Information</h3>");
        sb.AppendLine("                <div class='detail-row'>");
        sb.AppendLine("                    <div class='detail-label'>Suggested Invoice Number:</div>");
        sb.AppendLine($"                    <div class='detail-value'>INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("                <div class='detail-row'>");
        sb.AppendLine("                    <div class='detail-label'>Service Period:</div>");
        sb.AppendLine($"                    <div class='detail-value'>{@event.StartDate:MMM dd, yyyy} - {@event.EndDate:MMM dd, yyyy}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("                <div class='detail-row'>");
        sb.AppendLine("                    <div class='detail-label'>Services Provided:</div>");
        sb.AppendLine("                    <div class='detail-value'>Medical First Aid Coverage & Emergency Response</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        
        // Action Items
        sb.AppendLine("            <div class='action-required'>");
        sb.AppendLine("                <h3>? Action Items for Financial Department</h3>");
        sb.AppendLine("                <ol>");
        sb.AppendLine("                    <li><strong>Review Event Details:</strong> Verify all information is correct for invoicing</li>");
        sb.AppendLine("                    <li><strong>Calculate Charges:</strong> Determine appropriate charges based on service duration and type</li>");
        sb.AppendLine("                    <li><strong>Prepare Invoice:</strong> Create formal invoice with company letterhead and terms</li>");
        sb.AppendLine("                    <li><strong>Send to Customer:</strong> Email invoice to customer contact information provided above</li>");
        sb.AppendLine("                    <li><strong>Update Records:</strong> Mark invoice as sent in the financial system</li>");
        sb.AppendLine("                </ol>");
        sb.AppendLine("            </div>");
        
        // Important Notes
        sb.AppendLine("            <h3>?? Important Notes</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Payment Terms:</strong> Use standard Net 30 days payment terms</li>");
        sb.AppendLine("                <li><strong>Service Documentation:</strong> Include details of medical staff provided and hours covered</li>");
        sb.AppendLine("                <li><strong>Equipment Charges:</strong> Include any special equipment or supplies used</li>");
        sb.AppendLine("                <li><strong>Customer Copy:</strong> Send copy to event contact person for their records</li>");
        sb.AppendLine("                <li><strong>Follow-up:</strong> Schedule reminder emails if payment is not received within terms</li>");
        sb.AppendLine("            </ul>");
        
        // Contact Information for Questions
        sb.AppendLine("            <h3>? Questions About This Event?</h3>");
        sb.AppendLine("            <p>If you need additional information about this event or the services provided, please contact:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? Email: <a href='mailto:events@medicalfirstaid.com'>events@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Phone: +1 (555) 123-4567</div>");
        sb.AppendLine("                <div>?? Office Hours: Monday-Friday, 8:00 AM - 6:00 PM</div>");
        sb.AppendLine("            </div>");
        
        // System Information
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div class='detail-label'>System Reference:</div>");
        sb.AppendLine($"                <div class='detail-value'>Event Management System - ID: {@event.Id}</div>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p><strong>Please process this invoice request at your earliest convenience.</strong></p>");
        sb.AppendLine("            <p>Thank you for your prompt attention to this matter.</p>");
        
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class='footer'>");
        sb.AppendLine("            <p>This notification was automatically generated by the Medical First Aid Event Management System.</p>");
        sb.AppendLine($"            <p>Invoice request sent on {DateTime.UtcNow:MMMM dd, yyyy 'at' HH:mm} UTC</p>");
        sb.AppendLine($"            <p>Reference ID: EVENT-{@event.Id:D6} | Status: Send Invoice</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }
}