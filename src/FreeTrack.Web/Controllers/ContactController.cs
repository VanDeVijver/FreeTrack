using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;
using FreeTrack.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactInquiryService _inquiryService;
    private readonly IRoomService _roomService;
    private readonly IStudioServiceQueryService _serviceQueryService;

    public ContactController(
        IContactInquiryService inquiryService,
        IRoomService roomService,
        IStudioServiceQueryService serviceQueryService)
    {
        _inquiryService = inquiryService;
        _roomService = roomService;
        _serviceQueryService = serviceQueryService;
    }

    // Backs both "Vertel ons over je project" (hero) and "Contact Us" (room card) forms.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactFormViewModel form, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            // Re-render the full home page with the invalid form + validation messages,
            // rather than a bare error page — keeps the visitor on the same scroll story.
            var services = await _serviceQueryService.GetServicesAsync(ct);
            var rooms = await _roomService.GetRoomsAsync(ct);

            form.RoomOptions = rooms.Select(r => new ContactFormViewModel.RoomOption(r.Id, r.Name)).ToList();
            form.ServiceOptions = services.Select(s => new ContactFormViewModel.ServiceOption(s.Id, s.Category)).ToList();

            var vm = new HomeViewModel
            {
                Services = services.Select((s, i) => new ServiceViewModel
                {
                    Id = s.Id, Number = s.Number, Category = s.Category, Title = s.Title,
                    Description = s.Description, ImageUrl = s.ImageUrl, IsFeatured = i == 0
                }).ToList(),
                Rooms = rooms.Select((r, i) => new RoomViewModel
                {
                    Id = r.Id, Name = r.Name, Slug = r.Slug, BadgeLabel = r.BadgeLabel,
                    Subtitle = r.Subtitle, Description = r.Description, ImageUrl = r.ImageUrl,
                    DisplayIndex = i + 1, AccentClass = new[] { "tint-cool", "tint-amber", "tint-warm" }[i % 3],
                    ShowContactCta = i == rooms.Count - 1
                }).ToList(),
                ContactForm = form
            };
            // ContactController's default view lookup is Views/Contact/*, so point explicitly
            // at Home's Index view rather than duplicating the whole landing page markup.
            return View("~/Views/Home/Index.cshtml", vm);
        }

        var dto = new CreateContactInquiryDto
        {
            Name = form.Name,
            Email = form.Email,
            Phone = form.Phone,
            Message = form.Message,
            RoomId = form.RoomId,
            StudioServiceId = form.StudioServiceId,
            PreferredDate = form.PreferredDate
        };

        await _inquiryService.SubmitAsync(dto, ct);

        TempData["ContactSubmitted"] = true;
        return RedirectToAction("Index", "Home", new { }, fragment: "contact");
    }
}
