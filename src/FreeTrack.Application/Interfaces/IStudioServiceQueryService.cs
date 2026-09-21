using FreeTrack.Application.DTOs;

namespace FreeTrack.Application.Interfaces;

public interface IStudioServiceQueryService
{
    Task<IReadOnlyList<StudioServiceDto>> GetServicesAsync(CancellationToken ct = default);
}
