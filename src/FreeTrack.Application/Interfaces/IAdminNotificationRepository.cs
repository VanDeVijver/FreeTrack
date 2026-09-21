using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Interfaces;

public interface IAdminNotificationRepository
{
    Task AddAsync(AdminNotification notification, CancellationToken ct = default);
    Task<IReadOnlyList<AdminNotification>> GetUnreadAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
