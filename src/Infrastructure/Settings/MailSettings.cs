namespace Infrastructure.Settings;

public sealed class MailSettings
{
    public string EmailName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string SmtpHost { get; set; } = null!;
    public int SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public string SmtpLogin { get; set; } = null!;
    public string SmtpPassword { get; set; } = null!;
}