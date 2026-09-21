using FreeTrack.Domain.Enums;

namespace FreeTrack.Domain.Entities;

/// <summary>
/// A booking request for a Room. Never created as Approved — every booking
/// starts Pending and only becomes Approved through the admin workflow.
/// </summary>
public class Booking : BaseEntity
{
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? Message { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? AdminNote { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
}
