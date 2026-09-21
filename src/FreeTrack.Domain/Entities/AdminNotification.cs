namespace FreeTrack.Domain.Entities;

/// <summary>
/// In-app notification queue for the admin dashboard. Written alongside
/// (not instead of) the email attempt, so the approval workflow is visible
/// even before SMTP is configured.
/// </summary>
public class AdminNotification : BaseEntity
{
    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}
