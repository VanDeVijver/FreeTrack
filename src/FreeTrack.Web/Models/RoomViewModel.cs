namespace FreeTrack.Web.Models;

/// <summary>
/// One card in the "Find your frequency" / rooms grid.
/// AccentClass drives the color-tinted overlay per card (cool blue-gray for
/// Studio Lively, amber for Studio Poly, warm red for LOVIT in the reference
/// screenshots) — it's presentational, so it's assigned in the controller
/// rather than stored on the Room entity.
/// </summary>
public class RoomViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string BadgeLabel { get; init; } = string.Empty;   // "THE LIVEROOM"
    public string Subtitle { get; init; } = string.Empty;     // "45M² - LIVEROOM"
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public int DisplayIndex { get; init; }                    // 1, 2, 3 -> "01", "02", "03"
    public string AccentClass { get; init; } = "tint-cool";   // tint-cool | tint-amber | tint-warm
    public bool ShowContactCta { get; init; }                 // LOVIT-style "Contact Us" pill instead of arrow only

    public string DisplayNumber => DisplayIndex.ToString("D2");
}
