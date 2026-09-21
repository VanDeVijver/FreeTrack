using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Repositories;

public class AdminNotificationRepository : IAdminNotificationRepository
{
    private readonly AppDbContext _db;
    public AdminNotificationRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(AdminNotification notification, CancellationToken ct = default) =>
        await _db.AdminNotifications.AddAsync(notification, ct);

    public Task<IReadOnlyList<AdminNotification>> GetUnreadAsync(CancellationToken ct = default) =>
        _db.AdminNotifications.AsNoTracking().Where(n => !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<AdminNotification>)t.Result, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
