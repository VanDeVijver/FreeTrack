using FreeTrack.Application.DTOs;

namespace FreeTrack.Web.Models;

public class CalendarViewModel
{
    public CalendarMonthDto Calendar { get; init; } = null!;
    public string RoomSlug { get; init; } = string.Empty;
    public IReadOnlyList<RoomDto> AllRooms { get; init; } = new List<RoomDto>();
}
