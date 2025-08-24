# Email Notification Setup for Staff Assignments

This application automatically sends email notifications to staff members when they are assigned to shifts. The emails include detailed information about the event, shift details, and important instructions.

## Email Configuration

### 1. SMTP Settings

Configure your SMTP settings in `appsettings.json` or `appsettings.Development.json`:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Medical First Aid Manager",
    "EnableSsl": "true"
  }
}
```

### 2. Gmail Configuration

If using Gmail:

1. Enable 2-factor authentication on your Google account
2. Generate an "App Password" for the application
3. Use the app password (not your regular password) in `SmtpPassword`
4. Set `SmtpHost` to `smtp.gmail.com`
5. Set `SmtpPort` to `587`
6. Set `EnableSsl` to `true`

### 3. Other Email Providers

For other providers, configure the appropriate SMTP settings:

- **Outlook/Hotmail**: `smtp-mail.outlook.com`, port 587
- **Yahoo**: `smtp.mail.yahoo.com`, port 587
- **Office 365**: `smtp.office365.com`, port 587

## Email Templates

The email notification includes:

- **Event Details**: Name, location, duration, contact information
- **Shift Details**: Name, time, duration, description
- **Staff Role**: Color-coded badge showing their role
- **Important Notes**: Arrival instructions, required items
- **Professional HTML Formatting**: Mobile-responsive design

## Features

### Automatic Email Sending

- ? **Assignment Creation**: Email sent when staff is assigned to a shift
- ? **Rich HTML Content**: Professional formatting with event details
- ? **Mobile Responsive**: Works on all devices
- ? **Error Handling**: Email failures don't block assignment creation
- ? **Logging**: Detailed logs for troubleshooting

### Email Content

Each notification email includes:

1. **Personalized Greeting** with staff member's name
2. **Event Information**:
   - Event name and location
   - Event duration
   - Contact person and phone
   - Event description

3. **Shift Details**:
   - Shift name and time
   - Duration calculation
   - Shift description
   - Staff role with color coding

4. **Important Instructions**:
   - Arrive 15 minutes early
   - Bring certification documents
   - Dress code requirements
   - Contact information for questions

## Testing Email Configuration

To test your email configuration:

1. Assign a staff member to a shift
2. Check the application logs for email sending status
3. Verify the staff member receives the notification

## Troubleshooting

### Common Issues

1. **Email not sending**:
   - Check SMTP credentials
   - Verify network connectivity
   - Check firewall settings
   - Review application logs

2. **Gmail authentication errors**:
   - Ensure 2FA is enabled
   - Use app password, not regular password
   - Check Google account security settings

3. **Email in spam folder**:
   - Add sender to contacts
   - Configure SPF/DKIM records if using custom domain

### Logging

The application logs email activities:

- `Information`: Successful email sending
- `Warning`: Email settings not configured
- `Error`: Email sending failures

Check logs in the console or configure a logging provider to monitor email delivery.

## Security Considerations

- **Never commit real passwords** to source control
- Use **environment variables** or **Azure Key Vault** for production
- Consider using **managed identity** for cloud deployments
- **Encrypt sensitive configuration** data

## Disabling Email Notifications

To disable email notifications without removing the code:

1. Leave email settings empty in configuration
2. The service will log a warning and skip sending
3. Staff assignments will still work normally

## Production Deployment

For production environments:

1. Use **Azure SendGrid**, **AWS SES**, or similar managed service
2. Configure **environment variables** for sensitive settings
3. Set up **monitoring** for email delivery failures
4. Consider **rate limiting** to prevent spam

## Example Production Configuration

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUsername": "apikey",
    "SmtpPassword": "SG.your-sendgrid-api-key",
    "FromEmail": "noreply@yourcompany.com",
    "FromName": "Medical First Aid Manager",
    "EnableSsl": "true"
  }
}
```

This implementation provides a robust, professional email notification system that enhances communication with your medical first aid staff.