namespace FreeTrack.Application.DTOs;

public record StudioServiceDto(
    int Id,
    string Number,
    string Category,
    string Title,
    string Description,
    string ImageUrl,
    int SortOrder
);
