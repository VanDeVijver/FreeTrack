namespace FreeTrack.Domain.Entities;

/// <summary>
/// One of the offerings listed under "What we do" (Live & op locatie,
/// Studio & productie, Workshops & sessies).
/// Named StudioService (not "Service") to avoid clashing with the .NET DI
/// notion of a "service".
/// </summary>
public class StudioService : BaseEntity
{
    public string Number { get; set; } = string.Empty;    // "01"
    public string Category { get; set; } = string.Empty;  // "LIVE & OP LOCATIE"
    public string Title { get; set; } = string.Empty;     // "Make the room sound alive."
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ContactInquiry> Inquiries { get; set; } = new List<ContactInquiry>();
}
