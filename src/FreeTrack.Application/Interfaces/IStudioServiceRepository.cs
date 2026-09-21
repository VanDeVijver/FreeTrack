using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Interfaces;

public interface IStudioServiceRepository
{
    Task<IReadOnlyList<StudioService>> GetAllActiveAsync(CancellationToken ct = default);
    Task<StudioService?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(StudioService service, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
