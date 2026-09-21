using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _db;
    public RoomRepository(AppDbContext db) => _db = db;

    public Task<IReadOnlyList<Room>> GetAllActiveAsync(CancellationToken ct = default) =>
        _db.Rooms.Where(r => r.IsActive).AsNoTracking()
            .ToListAsync(ct).ContinueWith(t => (IReadOnlyList<Room>)t.Result, ct);

    public Task<Room?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        _db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Slug == slug, ct);

    public Task<Room?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Rooms.FindAsync(new object[] { id }, ct).AsTask();

    public async Task AddAsync(Room room, CancellationToken ct = default) =>
        await _db.Rooms.AddAsync(room, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
