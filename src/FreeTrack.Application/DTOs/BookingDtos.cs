using System.ComponentModel.DataAnnotations;

namespace FreeTrack.Application.DTOs;

public record BookingDto(
    int Id,
    int RoomId,
    string RoomName,
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone,
    string? Message,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status,
    string? AdminNote,
    DateTime CreatedAt,
    DateTime? DecidedAtUtc
);

public class CreateBookingRequestDto
{
    [Required]
    public int RoomId { get; set; }

    [Required, StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? CustomerPhone { get; set; }

    [StringLength(2000)]
    public string? Message { get; set; }

    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }
}

/// <summary>Result of a booking request attempt — Success=false means it was rejected
/// before ever being saved (bad dates, confirmed conflict), not an admin decision.</summary>
public record BookingRequestResultDto(bool Success, string Message, BookingDto? Booking);

public enum CalendarDayStatus { Past, Available, Pending, Booked }

public record CalendarDayDto(DateTime Date, CalendarDayStatus Status);

public record CalendarMonthDto(int RoomId, string RoomName, int Year, int Month, IReadOnlyList<CalendarDayDto> Days);
