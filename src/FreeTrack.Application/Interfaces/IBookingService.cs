using FreeTrack.Application.DTOs;
using FreeTrack.Domain.Enums;

namespace FreeTrack.Application.Interfaces;

public interface IBookingService
{
    Task<CalendarMonthDto> GetCalendarAsync(int roomId, int year, int month, CancellationToken ct = default);
    Task<BookingRequestResultDto> RequestBookingAsync(CreateBookingRequestDto input, CancellationToken ct = default);
    Task<IReadOnlyList<BookingDto>> GetBookingsForAdminAsync(BookingStatus? status, CancellationToken ct = default);
    Task<BookingDto> ApproveAsync(int bookingId, string? adminNote, CancellationToken ct = default);
    Task<BookingDto> RejectAsync(int bookingId, string? adminNote, CancellationToken ct = default);
}
