using FreeTrack.Domain.Enums;

namespace FreeTrack.Domain.Entities;

/// <summary>
/// Submission from the "Vertel ons over je project" / "Contact Us" forms.
/// </summary>
public class ContactInquiry : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;

    public int? RoomId { get; set; }
    public Room? Room { get; set; }

    public int? StudioServiceId { get; set; }
    public StudioService? StudioService { get; set; }

    public DateTime? PreferredDate { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
}
