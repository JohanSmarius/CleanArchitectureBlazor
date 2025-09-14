namespace CleanArchitectureBlazor.Configuration;

/// <summary>
/// Configuration options for email settings
/// </summary>
public class EmailOptions
{
    public const string SectionName = "EmailSettings";

    /// <summary>
    /// SMTP server host
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// SMTP username for authentication
    /// </summary>
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password for authentication
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// Email address to send from
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Display name for sent emails
    /// </summary>
    public string FromName { get; set; } = "Medical First Aid Manager";

    /// <summary>
    /// Whether to enable SSL for SMTP connection
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Email address for the financial department to receive invoice notifications
    /// </summary>
    public string FinancialDepartmentEmail { get; set; } = string.Empty;
}