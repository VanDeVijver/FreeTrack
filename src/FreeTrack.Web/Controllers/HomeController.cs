using FreeTrack.Application.Interfaces;
using FreeTrack.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Web.Controllers;

public class HomeController : Controller
{
    private static readonly string[] RoomTints = { "tint-cool", "tint-amber", "tint-warm" };

    private readonly IRoomService _roomService;
    private readonly IStudioServiceQueryService _serviceQueryService;

    public HomeController(IRoomService roomService, IStudioServiceQueryService serviceQueryService)
    {
        _roomService = roomService;
        _serviceQueryService = serviceQueryService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = await BuildViewModelAsync(ct);
        return View(vm);
    }

    private async Task<HomeViewModel> BuildViewModelAsync(CancellationToken ct, ContactFormViewModel? contactForm = null)
    {
        var services = await _serviceQueryService.GetServicesAsync(ct);
        var rooms = await _roomService.GetRoomsAsync(ct);

        var serviceVms = services.Select((s, i) => new ServiceViewModel
        {
            Id = s.Id,
            Number = s.Number,
            Category = s.Category,
            Title = s.Title,
            Description = s.Description,
            ImageUrl = s.ImageUrl,
            IsFeatured = i == 0
        }).ToList();

        var roomVms = rooms.Select((r, i) => new RoomViewModel
        {
            Id = r.Id,
            Name = r.Name,
            Slug = r.Slug,
            BadgeLabel = r.BadgeLabel,
            Subtitle = r.Subtitle,
            Description = r.Description,
            ImageUrl = r.ImageUrl,
            DisplayIndex = i + 1,
            AccentClass = RoomTints[i % RoomTints.Length],
            ShowContactCta = i == rooms.Count - 1 // last card mirrors the LOVIT "Contact Us" treatment
        }).ToList();

        var form = contactForm ?? new ContactFormViewModel();
        form.RoomOptions = roomVms.Select(r => new ContactFormViewModel.RoomOption(r.Id, r.Name)).ToList();
        form.ServiceOptions = serviceVms.Select(s => new ContactFormViewModel.ServiceOption(s.Id, s.Category)).ToList();

        return new HomeViewModel
        {
            Services = serviceVms,
            Rooms = roomVms,
            ContactForm = form
        };
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
