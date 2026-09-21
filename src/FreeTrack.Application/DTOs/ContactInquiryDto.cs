using System.ComponentModel.DataAnnotations;

namespace FreeTrack.Application.DTOs;

/// <summary>Input model for the "Vertel ons over je project" / "Contact Us" forms.</summary>
public class CreateContactInquiryDto
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [Required, StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public int? RoomId { get; set; }
    public int? StudioServiceId { get; set; }
    public DateTime? PreferredDate { get; set; }
}

public record ContactInquiryDto(
    int Id,
    string Name,
    string Email,
    string? Phone,
    string Message,
    int? RoomId,
    int? StudioServiceId,
    DateTime? PreferredDate,
    string Status,
    DateTime CreatedAt
);
