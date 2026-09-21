using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactInquiryService _inquiryService;
    public ContactController(IContactInquiryService inquiryService) => _inquiryService = inquiryService;

    // POST /api/contact -> "Vertel ons over je project" / "Contact Us" forms
    [HttpPost]
    public async Task<IActionResult> Submit(CreateContactInquiryDto input, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _inquiryService.SubmitAsync(input, ct);
        return CreatedAtAction(nameof(Submit), new { id = result.Id }, result);
    }
}
