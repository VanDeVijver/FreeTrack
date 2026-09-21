using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Interfaces;

/// <summary>
/// Notifies people when booking state changes. Implementations must never let a
/// notification failure (e.g. SMTP down) break the booking flow itself.
/// </summary>
public interface IBookingNotificationService
{
    Task NotifyAdminNewRequestAsync(Booking booking, CancellationToken ct = default);
    Task NotifyCustomerDecisionAsync(Booking booking, CancellationToken ct = default);
}
