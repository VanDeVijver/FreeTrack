namespace FreeTrack.Infrastructure.Options;

/// <summary>Bound from the "Smtp" appsettings section. Leave Host empty to disable
/// real email sending — notifications still get written to AdminNotifications either way.</summary>
public class SmtpOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }
    public string? AdminEmail { get; set; }
    public bool EnableSsl { get; set; } = true;
}
