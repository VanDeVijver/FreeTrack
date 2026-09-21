namespace FreeTrack.Infrastructure.Options;

/// <summary>Bound from the "Email" appsettings section (Resend API). Leave ApiKey empty to disable
/// real email sending — notifications still get written to AdminNotifications either way.</summary>
public class EmailOptions
{
    public string? ApiKey { get; set; }
    /// <summary>"Name &lt;address&gt;". Resend only allows onboarding@resend.dev (to your own signup address) until a domain is verified.</summary>
    public string FromAddress { get; set; } = "FreeTrack <onboarding@resend.dev>";
    public string? AdminEmail { get; set; }
}
