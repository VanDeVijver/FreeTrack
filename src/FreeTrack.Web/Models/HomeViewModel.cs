namespace FreeTrack.Web.Models;

/// <summary>Composed view model for the single-page marketing site (Views/Home/Index.cshtml).</summary>
public class HomeViewModel
{
    // Hero copy is static/editorial content, not DB-backed yet — kept here so it's
    // still easy to move into a SiteContent table later without touching the view.
    public string HeroKicker { get; init; } = "FREETRACK / WETTEREN";
    public string HeroHeadingPlain { get; init; } = "Sound is a";
    public string HeroHeadingAccent { get; init; } = "place.";
    public string HeroIntro { get; init; } =
        "Een studio, werkplaats en creatieve uitvalsbasis voor makers die hun idee niet kleiner willen maken dan het is.";

    public IReadOnlyList<string> TickerLabels { get; init; } =
        new[] { "LISTEN CLOSELY", "MAKE IT LOUD", "STAY CURIOUS", "FROM THE WOODS" };

    public IReadOnlyList<ServiceViewModel> Services { get; init; } = new List<ServiceViewModel>();
    public IReadOnlyList<RoomViewModel> Rooms { get; init; } = new List<RoomViewModel>();

    public ContactFormViewModel ContactForm { get; set; } = new();
    public bool ContactSubmitted { get; init; }
}
