using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Repositories;

public class StudioServiceRepository : IStudioServiceRepository
{
    private readonly AppDbContext _db;
    public StudioServiceRepository(AppDbContext db) => _db = db;

    public Task<IReadOnlyList<StudioService>> GetAllActiveAsync(CancellationToken ct = default) =>
        _db.StudioServices.Where(s => s.IsActive).AsNoTracking()
            .ToListAsync(ct).ContinueWith(t => (IReadOnlyList<StudioService>)t.Result, ct);

    public Task<StudioService?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.StudioServices.FindAsync(new object[] { id }, ct).AsTask();

    public async Task AddAsync(StudioService service, CancellationToken ct = default) =>
        await _db.StudioServices.AddAsync(service, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
