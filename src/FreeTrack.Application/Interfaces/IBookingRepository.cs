using FreeTrack.Domain.Entities;
using FreeTrack.Domain.Enums;

namespace FreeTrack.Application.Interfaces;

public interface IBookingRepository
{
    /// <summary>Tracked entity — use for approve/reject where you'll mutate and save.</summary>
    Task<Booking?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Non-cancelled/non-rejected bookings for a room overlapping [startUtc, endUtc).</summary>
    Task<IReadOnlyList<Booking>> GetForRoomInRangeAsync(int roomId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default);

    Task<IReadOnlyList<Booking>> GetByStatusAsync(BookingStatus? status, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
