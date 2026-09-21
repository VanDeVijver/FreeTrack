using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Enums;
using FreeTrack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IAdminNotificationRepository _notifications;

    public AdminController(IBookingService bookingService, IAdminNotificationRepository notifications)
    {
        _bookingService = bookingService;
        _notifications = notifications;
    }

    // GET /Admin?status=Pending
    [HttpGet("")]
    public async Task<IActionResult> Index(string status = "Pending", CancellationToken ct = default)
    {
        BookingStatus? filter = status switch
        {
            "Approved" => BookingStatus.Approved,
            "Rejected" => BookingStatus.Rejected,
            "All" => null,
            _ => BookingStatus.Pending
        };

        var bookings = await _bookingService.GetBookingsForAdminAsync(filter, ct);
        var unread = await _notifications.GetUnreadAsync(ct);

        return View(new AdminDashboardViewModel
        {
            StatusFilter = status,
            Bookings = bookings,
            UnreadNotificationCount = unread.Count
        });
    }

    [HttpPost("Approve/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? adminNote, CancellationToken ct)
    {
        try
        {
            await _bookingService.ApproveAsync(id, adminNote, ct);
            TempData["AdminMessage"] = "Boeking goedgekeurd en klant is op de hoogte gebracht.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["AdminError"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Reject/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string? adminNote, CancellationToken ct)
    {
        await _bookingService.RejectAsync(id, adminNote, ct);
        TempData["AdminMessage"] = "Boeking geweigerd en klant is op de hoogte gebracht.";
        return RedirectToAction(nameof(Index));
    }
}
