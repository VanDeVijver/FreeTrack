using FreeTrack.Application.DTOs;

namespace FreeTrack.Web.Models;

public class AdminDashboardViewModel
{
    public string StatusFilter { get; init; } = "Pending";
    public IReadOnlyList<BookingDto> Bookings { get; init; } = new List<BookingDto>();
    public int UnreadNotificationCount { get; init; }
}
