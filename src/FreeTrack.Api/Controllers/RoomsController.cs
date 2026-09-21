using FreeTrack.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    public RoomsController(IRoomService roomService) => _roomService = roomService;

    // GET /api/rooms  -> powers the "Find your frequency" grid
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _roomService.GetRoomsAsync(ct));

    // GET /api/rooms/studio-lively -> powers a room detail page
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var room = await _roomService.GetRoomBySlugAsync(slug, ct);
        return room is null ? NotFound() : Ok(room);
    }
}
