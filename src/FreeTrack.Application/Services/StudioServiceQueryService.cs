using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;

namespace FreeTrack.Application.Services;

public class StudioServiceQueryService : IStudioServiceQueryService
{
    private readonly IStudioServiceRepository _repository;

    public StudioServiceQueryService(IStudioServiceRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<StudioServiceDto>> GetServicesAsync(CancellationToken ct = default)
    {
        var services = await _repository.GetAllActiveAsync(ct);
        return services
            .OrderBy(s => s.SortOrder)
            .Select(s => new StudioServiceDto(s.Id, s.Number, s.Category, s.Title,
                s.Description, s.ImageUrl, s.SortOrder))
            .ToList();
    }
}
