using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Domain.Enums;

namespace FreeTrack.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IRoomRepository _rooms;
    private readonly IBookingNotificationService _notifications;

    public BookingService(IBookingRepository bookings, IRoomRepository rooms, IBookingNotificationService notifications)
    {
        _bookings = bookings;
        _rooms = rooms;
        _notifications = notifications;
    }

    public async Task<CalendarMonthDto> GetCalendarAsync(int roomId, int year, int month, CancellationToken ct = default)
    {
        var room = await _rooms.GetByIdAsync(roomId, ct)
            ?? throw new KeyNotFoundException($"Room {roomId} not found.");

        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);
        var bookings = await _bookings.GetForRoomInRangeAsync(roomId, monthStart, monthEnd, ct);
        var today = DateTime.UtcNow.Date;

        var days = new List<CalendarDayDto>();
        for (var day = monthStart; day < monthEnd; day = day.AddDays(1))
        {
            var dayEnd = day.AddDays(1);
            var overlapping = bookings.Where(b => b.StartUtc < dayEnd && b.EndUtc > day).ToList();

            var status =
                day < today ? CalendarDayStatus.Past :
                overlapping.Any(b => b.Status == BookingStatus.Approved) ? CalendarDayStatus.Booked :
                overlapping.Any(b => b.Status == BookingStatus.Pending) ? CalendarDayStatus.Pending :
                CalendarDayStatus.Available;

            days.Add(new CalendarDayDto(day, status));
        }

        return new CalendarMonthDto(room.Id, room.Name, year, month, days);
    }

    public async Task<BookingRequestResultDto> RequestBookingAsync(CreateBookingRequestDto input, CancellationToken ct = default)
    {
        if (input.EndUtc <= input.StartUtc)
            return new BookingRequestResultDto(false, "De einddatum moet na de startdatum liggen.", null);

        if (input.StartUtc.Date < DateTime.UtcNow.Date)
            return new BookingRequestResultDto(false, "Je kan geen datum in het verleden aanvragen.", null);

        var room = await _rooms.GetByIdAsync(input.RoomId, ct);
        if (room is null)
            return new BookingRequestResultDto(false, "Onbekende ruimte.", null);

        var existing = await _bookings.GetForRoomInRangeAsync(input.RoomId, input.StartUtc, input.EndUtc, ct);

        // A confirmed booking is a hard block. An overlapping *pending* request is not —
        // multiple people can ask for the same slot; the admin decides who gets it.
        var hasConfirmedConflict = existing.Any(b => b.Status == BookingStatus.Approved);
        if (hasConfirmedConflict)
            return new BookingRequestResultDto(false, "Deze periode is al bevestigd geboekt. Kies een andere datum.", null);

        var booking = new Booking
        {
            RoomId = input.RoomId,
            CustomerName = input.CustomerName,
            CustomerEmail = input.CustomerEmail,
            CustomerPhone = input.CustomerPhone,
            Message = input.Message,
            StartUtc = input.StartUtc,
            EndUtc = input.EndUtc,
            Status = BookingStatus.Pending
        };

        await _bookings.AddAsync(booking, ct);
        await _bookings.SaveChangesAsync(ct);
        booking.Room = room;

        await _notifications.NotifyAdminNewRequestAsync(booking, ct);

        var hasPendingOverlap = existing.Any(b => b.Status == BookingStatus.Pending);
        var message = hasPendingOverlap
            ? "Je aanvraag is verstuurd. Let op: er ligt al een andere aanvraag voor (een deel van) deze periode — we bevestigen op basis van beschikbaarheid."
            : "Je aanvraag is verstuurd. We nemen zo snel mogelijk contact met je op — dit is nog geen bevestiging.";

        return new BookingRequestResultDto(true, message, ToDto(booking));
    }

    public async Task<IReadOnlyList<BookingDto>> GetBookingsForAdminAsync(BookingStatus? status, CancellationToken ct = default)
    {
        var bookings = await _bookings.GetByStatusAsync(status, ct);
        return bookings.Select(ToDto).ToList();
    }

    public async Task<BookingDto> ApproveAsync(int bookingId, string? adminNote, CancellationToken ct = default)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, ct)
            ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

        var overlapping = await _bookings.GetForRoomInRangeAsync(booking.RoomId, booking.StartUtc, booking.EndUtc, ct);
        if (overlapping.Any(b => b.Id != booking.Id && b.Status == BookingStatus.Approved))
            throw new InvalidOperationException("Deze periode is al bevestigd voor een andere aanvraag.");

        booking.Status = BookingStatus.Approved;
        booking.AdminNote = adminNote;
        booking.DecidedAtUtc = DateTime.UtcNow;
        await _bookings.SaveChangesAsync(ct);

        await _notifications.NotifyCustomerDecisionAsync(booking, ct);
        return ToDto(booking);
    }

    public async Task<BookingDto> RejectAsync(int bookingId, string? adminNote, CancellationToken ct = default)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, ct)
            ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

        booking.Status = BookingStatus.Rejected;
        booking.AdminNote = adminNote;
        booking.DecidedAtUtc = DateTime.UtcNow;
        await _bookings.SaveChangesAsync(ct);

        await _notifications.NotifyCustomerDecisionAsync(booking, ct);
        return ToDto(booking);
    }

    private static BookingDto ToDto(Booking b) => new(
        b.Id, b.RoomId, b.Room?.Name ?? string.Empty, b.CustomerName, b.CustomerEmail,
        b.CustomerPhone, b.Message, b.StartUtc, b.EndUtc, b.Status.ToString(), b.AdminNote,
        b.CreatedAt, b.DecidedAtUtc);
}
