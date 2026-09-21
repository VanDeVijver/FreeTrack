using FreeTrack.Application.DTOs;

namespace FreeTrack.Application.Interfaces;

public interface IRoomService
{
    Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken ct = default);
    Task<RoomDto?> GetRoomBySlugAsync(string slug, CancellationToken ct = default);
}
