using CleanArchitectureBlazor.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using Domain;

namespace CleanArchitectureBlazor.Services;

/// <summary>
/// SMTP-based email service for sending notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions;
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
        if (string.IsNullOrEmpty(_emailOptions.Value.FinancialDepartmentEmail))
        {
            _logger.LogWarning("Cannot send event invoice email for event {EventId}: No financial email provided", @event.Id);
            return;
        }

        var subject = $"Invoice for Event: {@event.Name}";
        var htmlBody = GenerateEventInvoiceEmailBody(@event);
        
        await SendEmailAsync(_emailOptions.Value.FinancialDepartmentEmail, subject, htmlBody);
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

            if (string.IsNullOrEmpty(host)) // || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email settings not configured. Skipping email to {Email}", to);
                return;
            }

            using var client = new SmtpClient(host, port);

            if (smtpConfig.GetValue<bool>("EnableSsl", true))
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
        sb.AppendLine("                <h3>?? Your Event is Confirmed!</h3>");
        sb.AppendLine("                <p>We are pleased to confirm that medical first aid coverage has been arranged for your event.</p>");
        sb.AppendLine("            </div>");
        
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
        sb.AppendLine($"                <div>??? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} day(s), {duration.Hours} hour(s)" 
            : $"{duration.Hours} hour(s), {duration.Minutes} minute(s)";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duration:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-success'>? Confirmed</span></div>");
        sb.AppendLine("            </div>");
        
        // What's Next Section
        sb.AppendLine("            <h3>?? What Happens Next?</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Staff Assignment:</strong> Our team will assign qualified medical first aid personnel to your event</li>");
        sb.AppendLine("                <li><strong>Pre-Event Contact:</strong> You will be contacted 24-48 hours before the event with final details</li>");
        sb.AppendLine("                <li><strong>Day of Event:</strong> Our staff will arrive 30 minutes early for setup and briefing</li>");
        sb.AppendLine("                <li><strong>Equipment:</strong> All necessary medical equipment and supplies will be provided</li>");
        sb.AppendLine("            </ul>");
        
        // Important Notes
        sb.AppendLine("            <h3>?? Important Information</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Please ensure adequate parking and access for our medical team</li>");
        sb.AppendLine("                <li>Notify us immediately if there are any changes to your event details</li>");
        sb.AppendLine("                <li>Our team will need access to power outlets for medical equipment</li>");
        sb.AppendLine("                <li>A designated area for the first aid station should be available</li>");
        sb.AppendLine("            </ul>");
        
        // Contact Information
        sb.AppendLine("            <h3>?? Need to Make Changes?</h3>");
        sb.AppendLine("            <p>If you need to make any changes to your event or have questions, please contact us as soon as possible:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? Email: <a href='mailto:events@medicalfirstaid.com'>events@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Phone: +1 (555) 123-4567</div>");
        sb.AppendLine("                <div>?? Office Hours: Monday-Friday, 8:00 AM - 6:00 PM</div>");
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
        sb.AppendLine($"                <div>?? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} day(s), {duration.Hours} hour(s)" 
            : $"{duration.Hours} hour(s), {duration.Minutes} minute(s)";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Duration:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Current Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Planned</span></div>");
        sb.AppendLine("            </div>");
        
        // What's Next Section
        sb.AppendLine("            <h3>?? What Happens Next?</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Planning Phase:</strong> Our team will finalize the staffing and coverage plan for your event</li>");
        sb.AppendLine("                <li><strong>Staff Assignment:</strong> Qualified medical personnel will be assigned to cover your event</li>");
        sb.AppendLine("                <li><strong>Final Confirmation:</strong> You will receive a confirmation email once everything is confirmed</li>");
        sb.AppendLine("                <li><strong>Pre-Event Contact:</strong> We will contact you 24-48 hours before the event with final details</li>");
        sb.AppendLine("            </ul>");
        
        // Important Information
        sb.AppendLine("            <h3>?? Important Information</h3>");
        sb.AppendLine("            <p>While we are planning your event coverage, please ensure:</p>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Event details remain accurate - notify us immediately of any changes</li>");
        sb.AppendLine("                <li>Adequate parking and access will be available for our medical team</li>");
        sb.AppendLine("                <li>Power outlets will be accessible for medical equipment</li>");
        sb.AppendLine("                <li>A suitable area can be designated for the first aid station</li>");
        sb.AppendLine("            </ul>");
        
        // Estimated Timeline
        sb.AppendLine("            <h3>? Expected Timeline</h3>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? <strong>Planning Phase:</strong> Currently in progress</div>");
        sb.AppendLine("                <div>? <strong>Confirmation:</strong> Within 2-3 business days</div>");
        sb.AppendLine("                <div>?? <strong>Pre-Event Contact:</strong> 24-48 hours before event</div>");
        sb.AppendLine("            </div>");
        
        // Contact Information
        sb.AppendLine("            <h3>?? Need to Make Changes or Have Questions?</h3>");
        sb.AppendLine("            <p>If you need to make any changes to your event or have questions about our planning process, please contact us immediately:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? Email: <a href='mailto:events@medicalfirstaid.com'>events@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Phone: +1 (555) 123-4567</div>");
        sb.AppendLine("                <div>?? Office Hours: Monday-Friday, 8:00 AM - 6:00 PM</div>");
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
        sb.AppendLine("        .invoice-info { background: #f8f9fa; padding: 15px; border-radius: 4px; margin: 15px 0; }");
        sb.AppendLine("        .amount { font-size: 1.2em; font-weight: bold; color: #6f42c1; }");
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
        sb.AppendLine($"                <div class='detail-label'>Event Date & Time:</div>");
        sb.AppendLine($"                <div>??? {@event.StartDate:dddd, MMMM dd, yyyy HH:mm} - {@event.EndDate:dddd, MMMM dd, yyyy HH:mm}</div>");
        sb.AppendLine("            </div>");
        
        var duration = @event.EndDate - @event.StartDate;
        var durationText = duration.TotalDays >= 1 
            ? $"{duration.Days} day(s), {duration.Hours} hour(s)" 
            : $"{duration.Hours} hour(s), {duration.Minutes} minute(s)";
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Duration:</div>");
        sb.AppendLine($"                <div>?? {durationText}</div>");
        sb.AppendLine("            </div>");
        
        if (!string.IsNullOrEmpty(@event.Description))
        {
            sb.AppendLine("            <div class='detail-row'>");
            sb.AppendLine($"                <div class='detail-label'>Event Description:</div>");
            sb.AppendLine($"                <div>{@event.Description}</div>");
            sb.AppendLine("            </div>");
        }
        
        // Status Badge
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine($"                <div class='detail-label'>Event Status:</div>");
        sb.AppendLine($"                <div><span class='badge badge-primary'>?? Invoice Sent</span></div>");
        sb.AppendLine("            </div>");
        
        // Invoice Information
        sb.AppendLine("            <h3>?? Invoice Information</h3>");
        sb.AppendLine("            <div class='invoice-info'>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Invoice Number:</div>");
        sb.AppendLine($"                    <div>INV-{@event.Id:D6}-{DateTime.UtcNow:yyyyMM}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Invoice Date:</div>");
        sb.AppendLine($"                    <div>{DateTime.UtcNow:MMMM dd, yyyy}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine($"                <div class='detail-row'>");
        sb.AppendLine($"                    <div class='detail-label'>Service Period:</div>");
        sb.AppendLine($"                    <div>{@event.StartDate:MMM dd, yyyy} - {@event.EndDate:MMM dd, yyyy}</div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        
        // Services Provided Section
        sb.AppendLine("            <h3>?? Services Provided</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li><strong>Medical First Aid Coverage:</strong> Professional medical staff on-site during event</li>");
        sb.AppendLine("                <li><strong>Emergency Response:</strong> Immediate medical assistance for any incidents</li>");
        sb.AppendLine("                <li><strong>Medical Equipment:</strong> Full first aid kit and emergency medical supplies</li>");
        sb.AppendLine("                <li><strong>Staff Expertise:</strong> Certified medical professionals and first aiders</li>");
        sb.AppendLine("                <li><strong>Documentation:</strong> Incident reports and medical logs as required</li>");
        sb.AppendLine("            </ul>");
        
        // Payment Instructions
        sb.AppendLine("            <h3>?? Payment Information</h3>");
        sb.AppendLine("            <div class='highlight'>");
        sb.AppendLine("                <p><strong>Payment Terms:</strong> Net 30 days from invoice date</p>");
        sb.AppendLine("                <p><strong>Due Date:</strong> " + DateTime.UtcNow.AddDays(30).ToString("MMMM dd, yyyy") + "</p>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div><strong>Payment Methods Accepted:</strong></div>");
        sb.AppendLine("                <div>");
        sb.AppendLine("                    <ul>");
        sb.AppendLine("                        <li>?? Credit Card (Visa, MasterCard, AmEx)</li>");
        sb.AppendLine("                        <li>?? Bank Transfer</li>");
        sb.AppendLine("                        <li>?? Check (made payable to Medical First Aid Services)</li>");
        sb.AppendLine("                        <li>?? Online Payment Portal</li>");
        sb.AppendLine("                    </ul>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        
        // Contact Information for Invoice Queries
        sb.AppendLine("            <h3>?? Invoice Questions?</h3>");
        sb.AppendLine("            <p>If you have any questions about this invoice or need clarification on the services provided, please contact our billing department:</p>");
        sb.AppendLine("            <div class='detail-row'>");
        sb.AppendLine("                <div>?? Email: <a href='mailto:billing@medicalfirstaid.com'>billing@medicalfirstaid.com</a></div>");
        sb.AppendLine("                <div>?? Phone: +1 (555) 123-4567 ext. 2</div>");
        sb.AppendLine("                <div>?? Billing Hours: Monday-Friday, 9:00 AM - 5:00 PM</div>");
        sb.AppendLine("            </div>");
        
        // Additional Notes
        sb.AppendLine("            <h3>?? Important Notes</h3>");
        sb.AppendLine("            <ul>");
        sb.AppendLine("                <li>Please include the invoice number on all payments</li>");
        sb.AppendLine("                <li>Late payment charges may apply after the due date</li>");
        sb.AppendLine("                <li>For recurring events, please contact us about volume discounts</li>");
        sb.AppendLine("                <li>We appreciate your business and look forward to serving you again</li>");
        sb.AppendLine("            </ul>");
        
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
}