using CleanArchitectureBlazor.Configuration;
using CleanArchitectureBlazor.Data;
using Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendStaffAssignmentNotificationAsync(Staff staff, Shift shift, Event @event)
    {
        var subject = $"Shift Assignment: {@event.Name}";
        var htmlBody = GenerateAssignmentEmailBody(staff, shift, @event);
        await SendEmailAsync(staff.Email, subject, htmlBody);
    }

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

            if (string.IsNullOrEmpty(host))
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
        }
    }

    private string GenerateAssignmentEmailBody(Staff staff, Shift shift, Event @event)
    {
        var sb = new StringBuilder();
        sb.Append("<html><body>");
        sb.Append($"<p>Hello <b>{staff.FullName}</b>, you have been assigned to {@event.Name} - shift {shift.Name}.</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }

    private string GenerateEventPlannedEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        sb.Append("<html><body>");
        sb.Append($"<p>Your event '{@event.Name}' is now Planned.</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }

    private string GenerateEventConfirmationEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        sb.Append("<html><body>");
        sb.Append($"<p>Your event '{@event.Name}' is Confirmed.</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }

    private string GenerateEventInvoiceEmailBody(Event @event)
    {
        var sb = new StringBuilder();
        sb.Append("<html><body>");
        sb.Append($"<p>Invoice ready for event '{@event.Name}'.</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }
}