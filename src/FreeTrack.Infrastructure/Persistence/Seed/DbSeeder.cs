using FreeTrack.Domain.Entities;

namespace FreeTrack.Infrastructure.Persistence.Seed;

/// <summary>Seeds the content that mirrors what's currently hardcoded in the React site.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.StudioServices.Any())
        {
            db.StudioServices.AddRange(
                new StudioService
                {
                    Number = "01", Category = "LIVE & OP LOCATIE",
                    Title = "Make the room sound alive.",
                    Description = "Techniek, opname en productie voor concerten, performances, bedrijfsevents en alles daartussen.",
                    ImageUrl = "/images/service-live.jpg", SortOrder = 1
                },
                new StudioService
                {
                    Number = "02", Category = "STUDIO & PRODUCTIE",
                    Title = "Give the idea a voice.",
                    Description = "Van een fluisterende stem tot een zaal die beweegt. We ontwerpen het geluid rond het verhaal, niet andersom.",
                    ImageUrl = "/images/service-studio.jpg", SortOrder = 2
                },
                new StudioService
                {
                    Number = "03", Category = "WORKSHOPS & SESSIES",
                    Title = "Learn by listening.",
                    Description = "Sessies en workshops voor makers die hun geluid beter willen leren kennen.",
                    ImageUrl = "/images/service-workshop.jpg", SortOrder = 3
                }
            );
        }

        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(
                new Room
                {
                    Name = "Studio Lively", Slug = "studio-lively", BadgeLabel = "THE LIVEROOM",
                    Subtitle = "45M² - LIVEROOM",
                    Description = "Voor live takes, repetities en kleine producties.",
                    AreaSquareMeters = 45, ImageUrl = "/images/room-lively.jpg", SortOrder = 1
                },
                new Room
                {
                    Name = "Studio Poly", Slug = "studio-poly", BadgeLabel = "THE FLEX ROOM",
                    Subtitle = "55M² - POLYVALENTE RUIMTE",
                    Description = "Flexibel voor opnames, workshops en productiedagen.",
                    AreaSquareMeters = 55, ImageUrl = "/images/room-poly.jpg", SortOrder = 2
                },
                new Room
                {
                    Name = "LOVIT", Slug = "lovit", BadgeLabel = "THE LOFT",
                    Subtitle = "100M² - LOFT MET KITCHENETTE",
                    Description = "Een plek om langer te blijven en groot te denken.",
                    AreaSquareMeters = 100, ImageUrl = "/images/room-lovit.jpg", SortOrder = 3
                }
            );
        }

        await db.SaveChangesAsync();
    }
}
