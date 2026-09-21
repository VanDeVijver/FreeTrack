using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Domain.Enums;
using FreeTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _db;
    public BookingRepository(AppDbContext db) => _db = db;

    public Task<Booking?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<IReadOnlyList<Booking>> GetForRoomInRangeAsync(int roomId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default) =>
        _db.Bookings.AsNoTracking()
            .Where(b => b.RoomId == roomId
                && b.Status != BookingStatus.Rejected
                && b.Status != BookingStatus.Cancelled
                && b.StartUtc < endUtc && b.EndUtc > startUtc)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<Booking>)t.Result, ct);

    public Task<IReadOnlyList<Booking>> GetByStatusAsync(BookingStatus? status, CancellationToken ct = default)
    {
        var query = _db.Bookings.Include(b => b.Room).AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        return query.OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<Booking>)t.Result, ct);
    }

    public async Task AddAsync(Booking booking, CancellationToken ct = default) =>
        await _db.Bookings.AddAsync(booking, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
