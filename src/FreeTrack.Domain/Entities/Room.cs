namespace FreeTrack.Domain.Entities;

/// <summary>
/// A bookable space ("Studio Lively", "Studio Poly", "LOVIT") shown in the
/// "Find your frequency" / "The Rooms" section of the site.
/// </summary>
public class Room : BaseEntity
{
    public string Name { get; set; } = string.Empty;          // "Studio Lively"
    public string Slug { get; set; } = string.Empty;          // "studio-lively"
    public string BadgeLabel { get; set; } = string.Empty;    // "THE LIVEROOM"
    public string Subtitle { get; set; } = string.Empty;      // "45M² - LIVEROOM"
    public string Description { get; set; } = string.Empty;   // "Voor live takes, repetities en kleine producties."
    public int? AreaSquareMeters { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ContactInquiry> Inquiries { get; set; } = new List<ContactInquiry>();
}
