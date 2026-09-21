namespace FreeTrack.Application.DTOs;

public record RoomDto(
    int Id,
    string Name,
    string Slug,
    string BadgeLabel,
    string Subtitle,
    string Description,
    int? AreaSquareMeters,
    string ImageUrl,
    int SortOrder
);
