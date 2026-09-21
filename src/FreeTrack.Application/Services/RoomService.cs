using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;

namespace FreeTrack.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repository;

    public RoomService(IRoomRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken ct = default)
    {
        var rooms = await _repository.GetAllActiveAsync(ct);
        return rooms
            .OrderBy(r => r.SortOrder)
            .Select(r => new RoomDto(r.Id, r.Name, r.Slug, r.BadgeLabel, r.Subtitle,
                r.Description, r.AreaSquareMeters, r.ImageUrl, r.SortOrder))
            .ToList();
    }

    public async Task<RoomDto?> GetRoomBySlugAsync(string slug, CancellationToken ct = default)
    {
        var r = await _repository.GetBySlugAsync(slug, ct);
        if (r is null) return null;
        return new RoomDto(r.Id, r.Name, r.Slug, r.BadgeLabel, r.Subtitle,
            r.Description, r.AreaSquareMeters, r.ImageUrl, r.SortOrder);
    }
}
