namespace FreeTrack.Web.Models;

/// <summary>One row in the "What we do" numbered service list.</summary>
public class ServiceViewModel
{
    public int Id { get; init; }
    public string Number { get; init; } = string.Empty;   // "01"
    public string Category { get; init; } = string.Empty; // "LIVE & OP LOCATIE"
    public string Title { get; init; } = string.Empty;    // "Make the room sound alive."
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsFeatured { get; init; }                  // true for the row shown/expanded by default
}
