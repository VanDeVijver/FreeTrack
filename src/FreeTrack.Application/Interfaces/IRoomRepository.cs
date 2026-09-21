using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Interfaces;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Room?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Room?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Room room, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
