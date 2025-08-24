using CleanArchitectureBlazor.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// SMTP-based email service for sending notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
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
    /// Sends a general email using SMTP
    /// </summary>
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var smtpConfig = _configuration.GetSection("EmailSettings");
            var host = smtpConfig["SmtpHost"];
            var port = int.Parse(smtpConfig["SmtpPort"] ?? "587");
            var username = smtpConfig["SmtpUsername"];
            var password = smtpConfig["SmtpPassword"];
            var fromEmail = smtpConfig["FromEmail"];
            var fromName = smtpConfig["FromName"] ?? "Medical First Aid Manager";
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"] ?? "true");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", to);
                return;
            }

            using var client = new SmtpClient(host, port);
            client.EnableSsl = enableSsl;
            client.Credentials = new NetworkCredential(username, password);

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
        sb.AppendLine($"                <div>??? {@event.StartDate:MMM dd, yyyy HH:mm} - {@event.EndDate:MMM dd, yyyy HH:mm}</div>");
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
}