using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;
using FreeTrack.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Web.Controllers;

public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IRoomService _roomService;

    public BookingsController(IBookingService bookingService, IRoomService roomService)
    {
        _bookingService = bookingService;
        _roomService = roomService;
    }

    // GET /Bookings/Calendar (navbar entry point) -> first room's calendar
    [HttpGet("/Bookings/Calendar")]
    public async Task<IActionResult> CalendarIndex(CancellationToken ct)
    {
        var first = (await _roomService.GetRoomsAsync(ct)).FirstOrDefault();
        return first is null ? NotFound() : RedirectToAction(nameof(Calendar), new { roomSlug = first.Slug });
    }

    // GET /Bookings/Calendar/studio-lively?year=2026&month=10
    [HttpGet("/Bookings/Calendar/{roomSlug}")]
    public async Task<IActionResult> Calendar(string roomSlug, int? year, int? month, CancellationToken ct)
    {
        var room = await _roomService.GetRoomBySlugAsync(roomSlug, ct);
        if (room is null) return NotFound();

        var now = DateTime.UtcNow;
        var calendar = await _bookingService.GetCalendarAsync(room.Id, year ?? now.Year, month ?? now.Month, ct);
        var allRooms = await _roomService.GetRoomsAsync(ct);

        return View(new CalendarViewModel
        {
            Calendar = calendar,
            RoomSlug = roomSlug,
            AllRooms = allRooms
        });
    }

    // GET /Bookings/Request/studio-lively?date=2026-10-14
    [HttpGet("/Bookings/Request/{roomSlug}")]
    public async Task<IActionResult> Request(string roomSlug, DateTime? date, CancellationToken ct)
    {
        var room = await _roomService.GetRoomBySlugAsync(roomSlug, ct);
        if (room is null) return NotFound();

        var start = date ?? DateTime.UtcNow.Date.AddDays(1);

        return View(new BookingRequestViewModel
        {
            RoomId = room.Id,
            RoomSlug = roomSlug,
            RoomName = room.Name,
            StartDate = start,
            EndDate = start.AddDays(1)
        });
    }

    [HttpPost("/Bookings/Request")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Request(BookingRequestViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _bookingService.RequestBookingAsync(new CreateBookingRequestDto
        {
            RoomId = model.RoomId,
            CustomerName = model.CustomerName,
            CustomerEmail = model.CustomerEmail,
            CustomerPhone = model.CustomerPhone,
            Message = model.Message,
            StartUtc = model.StartDate,
            EndUtc = model.EndDate
        }, ct);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["BookingMessage"] = result.Message;
        return RedirectToAction(nameof(Confirmation), new { roomSlug = model.RoomSlug });
    }

    [HttpGet("/Bookings/Confirmation/{roomSlug}")]
    public IActionResult Confirmation(string roomSlug)
    {
        ViewBag.RoomSlug = roomSlug;
        ViewBag.Message = TempData["BookingMessage"] as string ?? "Je aanvraag is verstuurd.";
        return View();
    }
}
