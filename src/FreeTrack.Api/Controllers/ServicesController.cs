using FreeTrack.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IStudioServiceQueryService _serviceQueryService;
    public ServicesController(IStudioServiceQueryService serviceQueryService) => _serviceQueryService = serviceQueryService;

    // GET /api/services -> powers the "What we do" numbered list
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _serviceQueryService.GetServicesAsync(ct));
}
